using System;
using System.Collections.Generic;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionDataGenerationTests
{
    [Fact]
    public void CreateCollisionInfo_ObjectsOverlapAtZeroVelocity_IncludesState()
    {
        var cmd = new CreateCollisionInfoCommand(
            new[] { new Vector(0, 0) }, new[] { new Vector(0, 0) },
            new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 }
        );

        cmd.Execute();

        Assert.NotNull(cmd.RelativeStates);
        Assert.Contains((0, 0, 0, 0), cmd.RelativeStates);
    }

    [Fact]
    public void CreateCollisionInfo_WrongVectorDimensionInToPointsSet_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CreateCollisionInfoCommand(
            new[] { new Vector(0) }, 
            new[] { new Vector(0, 0) },
            new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 }
        ));
    }

    [Fact]
    public void CreateCollisionTreeCommand_CreatesValidTreeProperty()
    {
        var states = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        var cmd = new CreateCollisionTreeCommand(states);
        
        Assert.Null(cmd.Tree);

        cmd.Execute();

        Assert.NotNull(cmd.Tree);
        Assert.True(cmd.Tree.Contains((1, 2, 3, 4)));
    }
    
    [Fact]
    public void CreateCollisionInfo_NoCollision_ReturnsEmptyList()
    {
        var cmd = new CreateCollisionInfoCommand(
            new[] { new Vector(0, 0) }, new[] { new Vector(10, 10) },
            new[] { 1 }, new[] { 0 }, new[] { 1 }, new[] { 0 }
        );
        cmd.Execute();
        Assert.NotNull(cmd.RelativeStates);
        Assert.Empty(cmd.RelativeStates);
    }
}

