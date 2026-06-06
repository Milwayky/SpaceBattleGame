using System;
using System.Collections.Generic;
using App;
using App.Scopes;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class RegisterIoCTreeDependenciesTests
{
    private void Setup()
    {
        try { new InitCommand().Execute(); } catch { }
        try { new RegisterIoCTreeDependencies(new Dictionary<(string, string), CollisionTree>()).Execute(); } catch { }
    }

    [Fact]
    public void Execute_RegistersAllTreeDependencies_AndTheyResolveSuccessfully()
    {
        Setup();
        var quads = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        Assert.NotNull(Ioc.Resolve<object>("CollisionTree.Create", quads));
    }

    [Fact]
    public void StrategyTreeCreate_InvalidCast_ThrowsException()
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
    public void StrategyTreeAdd_MissingArgs_ThrowsException()
    {
        Setup();
        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<object>("Collision.Tree.Add", "only_one_arg"));
    }

    [Fact]
    public void IoC_Configuration_Coverage()
    {
        Setup();
        Assert.ThrowsAny<Exception>(() => Ioc.Resolve<object>("NonExistentDependency123"));
    }
}

