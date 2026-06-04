using System;

namespace SpaceBattle.Lib;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = true)]
public class AdapterAttribute : Attribute
{
    public Type InterfaceType { get; }
    public string? PropertyName { get; }
    public Type StrategyType { get; }
    public string MethodName { get; }

    public AdapterAttribute(Type interfaceType, string propertyName, Type strategyType, string methodName)
    {
        InterfaceType = interfaceType;
        PropertyName = propertyName;
        StrategyType = strategyType;
        MethodName = methodName;
    }
}

