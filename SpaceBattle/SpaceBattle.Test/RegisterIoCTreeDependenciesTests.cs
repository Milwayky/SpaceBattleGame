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
    private static bool _initialized = false;
    private static readonly object _lock = new();

    public RegisterIoCTreeDependenciesTests()
    {
        lock (_lock)
        {
            if (!_initialized)
            {
                new App.Scopes.InitCommand().Execute();
                var storage = new Dictionary<(string, string), CollisionTree>();
                new RegisterIoCTreeDependencies(storage).Execute();
                _initialized = true;
            }
        }
    }

    [Fact]
    public void Execute_RegistersAllTreeDependencies_AndTheyResolveSuccessfully()
    {
        var quads = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        var createTree = Ioc.Resolve<object>("CollisionTree.Create", quads);
        Assert.NotNull(createTree);

        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[] { "1,2,3,4" });
        try
        {
            var readFile = Ioc.Resolve<object>("CollisionTree.ReadFile", tempFile);
            Assert.NotNull(readFile);
        }
        finally { File.Delete(tempFile); }
    }

    [Fact]
    public void StrategyTreeCreate_InvalidCast_ThrowsException()
    {
        Assert.Throws<InvalidCastException>(() => Ioc.Resolve<object>("CollisionTree.Create", "not_a_list"));
    }

    [Fact]
    public void StrategyTreeReadFile_MissingArgs_ThrowsException()
    {
        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<object>("CollisionTree.ReadFile"));
    }

    [Fact]
    public void StrategyTreeAdd_MissingArgs_ThrowsException()
    {
        Assert.Throws<IndexOutOfRangeException>(() => Ioc.Resolve<object>("Collision.Tree.Add", "only_one_arg"));
    }

    [Fact]
    public void IoC_Configuration_Coverage()
    {
        Assert.ThrowsAny<Exception>(() => Ioc.Resolve<object>("NonExistentDependency"));
    }
}

