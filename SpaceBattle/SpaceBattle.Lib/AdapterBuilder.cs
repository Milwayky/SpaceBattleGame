using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SpaceBattle.Lib;

public static class AdapterBuilder
{
    private static readonly ModuleBuilder ModuleBuilder;
    private static readonly Dictionary<Type, Type> AdapterTypeCache = new();

    static AdapterBuilder()
    {
        var assemblyName = new AssemblyName("DynamicAdapters");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = assemblyBuilder.DefineDynamicModule("AdaptersModule");
    }

    public static TInterface Build<TInterface>(IDictionary<string, object> data) where TInterface : class
    {
        var adapterType = GetOrCreateAdapterType<TInterface>();
        return (TInterface)Activator.CreateInstance(adapterType, data)!;
    }

    public static object Build(Type interfaceType, IDictionary<string, object> data)
    {
        var adapterType = GetOrCreateAdapterType(interfaceType);
        return Activator.CreateInstance(adapterType, data)!;
    }

    private static Type GetOrCreateAdapterType<TInterface>() where TInterface : class
    {
        return GetOrCreateAdapterType(typeof(TInterface));
    }

    private static Type GetOrCreateAdapterType(Type interfaceType)
    {
        if (AdapterTypeCache.TryGetValue(interfaceType, out var cached))
            return cached;

        if (!interfaceType.IsInterface)
            throw new ArgumentException($"Type {interfaceType.Name} is not an interface.");

        var adapterType = GenerateAdapterType(interfaceType);
        AdapterTypeCache[interfaceType] = adapterType;
        return adapterType;
    }

    private static Type GenerateAdapterType(Type interfaceType)
    {
        var typeBuilder = ModuleBuilder.DefineType(
            $"{interfaceType.Name}_Adapter_{Guid.NewGuid():N}",
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(object),
            new[] { interfaceType }
        );

        var dataField = typeBuilder.DefineField(
            "_data",
            typeof(IDictionary<string, object>),
            FieldAttributes.Private | FieldAttributes.InitOnly
        );

        var ctorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            new[] { typeof(IDictionary<string, object>) }
        );
        var ctorIl = ctorBuilder.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Ldarg_1);
        ctorIl.Emit(OpCodes.Stfld, dataField);
        ctorIl.Emit(OpCodes.Ret);

        var allInterfaces = GetAllInterfaces(interfaceType);
        foreach (var iface in allInterfaces)
        {
            foreach (var property in iface.GetProperties())
            {
                ImplementProperty(typeBuilder, dataField, property, interfaceType);
            }
        }

        return typeBuilder.CreateType();
    }

    private static HashSet<Type> GetAllInterfaces(Type type)
    {
        var result = new HashSet<Type>();
        var queue = new Queue<Type>();
        queue.Enqueue(type);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (result.Add(current))
            {
                foreach (var iface in current.GetInterfaces())
                    queue.Enqueue(iface);
            }
        }
        return result;
    }

    private static void ImplementProperty(
        TypeBuilder typeBuilder,
        FieldInfo dataField,
        PropertyInfo property,
        Type adapterInterfaceType)
    {
        var propBuilder = typeBuilder.DefineProperty(
            property.Name,
            PropertyAttributes.None,
            property.PropertyType,
            Type.EmptyTypes
        );

        var adapterAttr = property.GetCustomAttribute<AdapterAttribute>();
        bool hasCustomStrategy = adapterAttr != null &&
            (adapterAttr.InterfaceType == adapterInterfaceType ||
             adapterAttr.InterfaceType.IsAssignableFrom(adapterInterfaceType)) &&
            adapterAttr.PropertyName == property.Name;

        if (property.CanRead)
        {
            var getter = typeBuilder.DefineMethod(
                $"get_{property.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName,
                property.PropertyType,
                Type.EmptyTypes
            );

            var il = getter.GetILGenerator();

            if (hasCustomStrategy && adapterAttr != null)
            {
                var method = adapterAttr.StrategyType.GetMethod(
                    adapterAttr.MethodName,
                    BindingFlags.Public | BindingFlags.Static
                );
                if (method == null)
                    throw new InvalidOperationException(
                        $"Method '{adapterAttr.MethodName}' not found in {adapterAttr.StrategyType.Name}");

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, dataField);

                il.Emit(OpCodes.Call, method);

                if (property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, property.PropertyType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, property.PropertyType);
                }
                il.Emit(OpCodes.Ret);
            }
            else
            {
                var containsKey = typeof(IDictionary<string, object>).GetMethod("ContainsKey")!;
                var getItem = typeof(IDictionary<string, object>).GetMethod("get_Item")!;

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, dataField);
                il.Emit(OpCodes.Ldstr, property.Name);
                il.Emit(OpCodes.Callvirt, containsKey);

                var notFound = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, notFound);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, dataField);
                il.Emit(OpCodes.Ldstr, property.Name);
                il.Emit(OpCodes.Callvirt, getItem);
                if (property.PropertyType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, property.PropertyType);
                else
                    il.Emit(OpCodes.Castclass, property.PropertyType);
                il.Emit(OpCodes.Ret);

                il.MarkLabel(notFound);
                il.Emit(OpCodes.Ldstr, $"Key '{property.Name}' not found in dictionary");
                il.Emit(OpCodes.Newobj, typeof(KeyNotFoundException).GetConstructor(new[] { typeof(string) })!);
                il.Emit(OpCodes.Throw);
            }

            propBuilder.SetGetMethod(getter);
        }

        if (property.CanWrite)
        {
            var setter = typeBuilder.DefineMethod(
                $"set_{property.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName,
                typeof(void),
                new[] { property.PropertyType }
            );

            var il = setter.GetILGenerator();
            var setItem = typeof(IDictionary<string, object>).GetMethod("set_Item")!;

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, dataField);
            il.Emit(OpCodes.Ldstr, property.Name);
            il.Emit(OpCodes.Ldarg_1);
            if (property.PropertyType.IsValueType)
                il.Emit(OpCodes.Box, property.PropertyType);
            il.Emit(OpCodes.Callvirt, setItem);
            il.Emit(OpCodes.Ret);

            propBuilder.SetSetMethod(setter);
        }
    }
}

