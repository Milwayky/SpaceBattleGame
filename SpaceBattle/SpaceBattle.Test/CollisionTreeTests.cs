using System.Collections.Generic;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionTreeTests
{
    [Fact]
    public void Constructor_WithRelativeStates_AddsAllStatesCorrectly()
    {
        var states = new List<(int, int, int, int)> { (1, 2, 3, 4), (5, 6, 7, 8) };
        var tree = new CollisionTree(states);

        Assert.True(tree.Contains((1, 2, 3, 4)));
        Assert.True(tree.Contains((5, 6, 7, 8)));
    }

    [Fact]
    public void Add_ExistingStatePathWithDifferentFinalElement_AppendsSuccessfully()
    {
        var tree = new CollisionTree(new List<(int, int, int, int)>());
        tree.Add((1, 2, 3, 4));
        tree.Add((1, 2, 3, 5));

        Assert.True(tree.Contains((1, 2, 3, 4)));
        Assert.True(tree.Contains((1, 2, 3, 5)));
    }

    [Fact]
    public void Contains_ReturnsFalse_ForEveryLevelOfMissingData()
    {
        var tree = new CollisionTree(new List<(int, int, int, int)> { (1, 2, 3, 4) });

        Assert.False(tree.Contains((99, 2, 3, 4))); 
        Assert.False(tree.Contains((1, 99, 3, 4))); 
        Assert.False(tree.Contains((1, 2, 99, 4))); 
        Assert.False(tree.Contains((1, 2, 3, 99)));
    }
}

