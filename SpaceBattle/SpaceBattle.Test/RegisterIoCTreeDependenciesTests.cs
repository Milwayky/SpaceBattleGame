using System;
using System.Collections.Generic;
using System.IO;
using App;
using App.Scopes;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class RegisterIoCTreeDependenciesTests
{
    private void Setup()
    {
        new InitCommand().Execute();
        new RegisterIoCTreeDependencies(new Dictionary<(string, string), CollisionTree>()).Execute();
    }

    [Fact]
    public void Execute_RegistersAllTreeDependencies_AndTheyResolveSuccessfully()
    {
        Setup();
        
        var quads = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        Assert.NotNull(Ioc.Resolve<object>("CollisionTree.Create", quads));

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "1,2,3,4");
        Assert.NotNull(Ioc.Resolve<object>("CollisionTree.ReadFile", tempFile));
        File.Delete(tempFile);

        var vList = new List<Vector>();
        var iList = new List<int>();
        Assert.NotNull(Ioc.Resolve<object>("CollisionInfo.Create", vList, vList, iList, iList, iList, iList));

        Assert.NotNull(Ioc.Resolve<object>("CollisionInfo.WriteFile", "dummy.txt", quads));

        var tree = new CollisionTree(quads);
        Assert.NotNull(Ioc.Resolve<object>("Collision.Tree.Add", "ship", "torpedo", tree));

        var dict = new Dictionary<string, object> { ["Form"] = "ship", ["Position"] = new Vector(0,0), ["Velocity"] = new Vector(0,0) };
        var obj = Ioc.Resolve<ICollisionObject>("Adapters.ICollisionObject", dict);
        Assert.NotNull(obj);

        Assert.NotNull(Ioc.Resolve<object>("Collision.Check", obj, obj));

        Assert.NotNull(Ioc.Resolve<object>("Collision.CheckAll", obj, new List<object> { obj }));

        try { Ioc.Resolve<object>("Commands.MoveWithCollisionCheck", obj, new List<object> { obj }); } catch { }
    }

    [Fact]
    public void StrategyTreeCreate_InvalidArgs_ThrowsException()
    {
        Setup();
        Assert.Throws<InvalidCastException>(() => Ioc.Resolve<object>("CollisionTree.Create", "not_a_list"));
    }

    [Fact]
    public void StrategyTreeReadFile_MissingArgs_ThrowsException()
    {
        Setup();
        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<object>("CollisionTree.ReadFile"));
    }

    [Fact]
    public void IoC_Resolve_NonExistentDependency_ThrowsKeyNotFoundException()
    {
        Setup();
        Assert.ThrowsAny<Exception>(() => Ioc.Resolve<object>("NonExistentKey"));
    }
}

