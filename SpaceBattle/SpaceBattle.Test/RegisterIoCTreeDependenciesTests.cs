using System;
using System.Collections.Generic;
using System.IO;
using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class RegisterIoCTreeDependenciesTests
{
    public RegisterIoCTreeDependenciesTests()
    {
        try
        {
            Ioc.Resolve<object>("IoC.Scope.Current");
        }
        catch
        {
            new InitCommand().Execute();
        }
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void Execute_RegistersAllTreeDependencies_AndTheyResolveSuccessfully()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        var regCommand = new RegisterIoCTreeDependencies(storage);
        regCommand.Execute();

        var quads = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        var createTree = Ioc.Resolve<SpaceBattle.Lib.ICommand>("CollisionTree.Create", quads);
        Assert.NotNull(createTree);

        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[] { "1,2,3,4" });
        try
        {
            var readFile = Ioc.Resolve<SpaceBattle.Lib.ICommand>("CollisionTree.ReadFile", tempFile);
            Assert.NotNull(readFile);
        }
        finally { File.Delete(tempFile); }

        var fakeTree = new CollisionTree(quads);
        var addCmd = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Collision.Tree.Add", "ship", "ufo", fakeTree);
        addCmd.Execute();
        Assert.True(storage.ContainsKey(("ship", "ufo")));

        var dictData = new Dictionary<string, object> { ["Form"] = "test", ["Position"] = new Vector(0, 0), ["Velocity"] = new Vector(0, 0) };
        var resolvedObj = Ioc.Resolve<ICollisionObject>("Adapters.ICollisionObject", dictData);
        Assert.NotNull(resolvedObj);
    }

    [Fact]
    public void StrategyTreeCreate_InvalidCast_ThrowsException()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        new RegisterIoCTreeDependencies(storage).Execute();

        Assert.Throws<InvalidCastException>(() => Ioc.Resolve<App.ICommand>("CollisionTree.Create", "not_a_list"));
    }

    [Fact]
    public void StrategyTreeReadFile_MissingArgs_ThrowsException()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        new RegisterIoCTreeDependencies(storage).Execute();

        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<App.ICommand>("CollisionTree.ReadFile"));
    }

    [Fact]
    public void StrategyTreeAdd_MissingArgs_ThrowsException()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        new RegisterIoCTreeDependencies(storage).Execute();

        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<App.ICommand>("Collision.Tree.Add", "only_one_arg"));
    }
}

