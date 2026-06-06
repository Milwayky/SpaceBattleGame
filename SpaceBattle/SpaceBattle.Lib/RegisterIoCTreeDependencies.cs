using System;
using System.Collections.Generic;
using System.Linq;
using App;

namespace SpaceBattle.Lib;

public class RegisterIoCTreeDependencies : ICommand
{
    private readonly IDictionary<(string, string), CollisionTree> _storage;

    public RegisterIoCTreeDependencies(IDictionary<(string, string), CollisionTree> storage) => _storage = storage;

    public void Execute()
    {
        SafeRegister("CollisionTree.Create", (object[] args) => new CreateCollisionTreeCommand((IEnumerable<(int, int, int, int)>)args[0]));
        SafeRegister("CollisionTree.ReadFile", (object[] args) => new ReadCollisionInfoFileCommand((string)args[0]));
        SafeRegister("CollisionInfo.Create", (object[] args) => new CreateCollisionInfoCommand((IEnumerable<Vector>)args[0], (IEnumerable<Vector>)args[1], (IEnumerable<int>)args[2], (IEnumerable<int>)args[3], (IEnumerable<int>)args[4], (IEnumerable<int>)args[5]));
        SafeRegister("CollisionInfo.WriteFile", (object[] args) => new WriteCollisionInfoFileCommand((string)args[0], (IEnumerable<(int, int, int, int)>)args[1]));
        SafeRegister("Collision.Tree.Add", (object[] args) => new AddToStorageCommand((string)args[0], (string)args[1], (CollisionTree)args[2], _storage));
        SafeRegister("Adapters.ICollisionObject", (object[] args) => new CollisionObject((IDictionary<string, object>)args[0]));
        
        SafeRegister("Collision.Check", (object[] args) => {
            var f = ResolveObj(args[0]); 
            var s = ResolveObj(args[1]);
            return new CheckCollisionsCommand(f, new[] { s }, _storage);
        });

        SafeRegister("Collision.CheckAll", (object[] args) => {
            var t = ResolveObj(args[0]); var o = ((IEnumerable<object>)args[1]).Select(ResolveObj).ToArray();
            return new CheckCollisionsCommand(t, o, _storage);
        });

        SafeRegister("Commands.MoveWithCollisionCheck", (object[] args) => {
            var move = Ioc.Resolve<ICommand>("Commands.Move", args[0]);
            var t = ResolveObj(args[0]); var o = ((IEnumerable<object>)args[1]).Select(ResolveObj).ToArray();
            var check = new CheckCollisionsCommand(t, o, _storage);
            return new MoveWithCollisionCheckCommand(move, check);
        });
    }

    private static void SafeRegister(string key, object strategy)
    {
        try
        {
            ((dynamic)Ioc.Resolve<object>("IoC.Register", key, strategy)).Execute();
        }
        catch (Exception) { }
    }

    private static ICollisionObject ResolveObj(object arg) => 
        arg is ICollisionObject obj ? obj : Ioc.Resolve<ICollisionObject>("Adapters.ICollisionObject", arg);
}

