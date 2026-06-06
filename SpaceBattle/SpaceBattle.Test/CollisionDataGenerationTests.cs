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
        var fPoints = new[] { new Vector(0, 0) };
        var sPoints = new[] { new Vector(0, 0) };
        
        var cmd = new CreateCollisionInfoCommand(fPoints, sPoints, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 });
        cmd.Execute();
        
        Assert.NotNull(cmd.RelativeStates);
        Assert.Single(cmd.RelativeStates);
    }

    [Fact]
    public void CreateCollisionInfo_NoCollision_ReturnsEmptyList()
    {
        var fPoints = new[] { new Vector(0, 0) };
        var sPoints = new[] { new Vector(10, 10) };
        
        var cmd = new CreateCollisionInfoCommand(fPoints, sPoints, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 });
        cmd.Execute();
        
        Assert.NotNull(cmd.RelativeStates);
        Assert.Empty(cmd.RelativeStates);
    }

    [Fact]
    public void CreateCollisionInfo_WithVelocityCollisionOnPath_ReturnsState()
    {
        var fPoints = new[] { new Vector(2, 2) };
        var sPoints = new[] { new Vector(0, 0) };
        
        var cmd = new CreateCollisionInfoCommand(fPoints, sPoints, new[] { 0 }, new[] { 0 }, new[] { 4 }, new[] { 4 });
        cmd.Execute();

        Assert.NotNull(cmd.RelativeStates);
        Assert.NotEmpty(cmd.RelativeStates);
    }

    [Fact]
    public void CreateCollisionInfo_WithVelocityNoCollisionOnPath_ReturnsEmpty()
    {
        var fPoints = new[] { new Vector(10, 10) };
        var sPoints = new[] { new Vector(0, 0) };
        
        var cmd = new CreateCollisionInfoCommand(fPoints, sPoints, new[] { 0 }, new[] { 0 }, new[] { 1 }, new[] { 1 });
        cmd.Execute();

        Assert.NotNull(cmd.RelativeStates);
        Assert.Empty(cmd.RelativeStates);
    }

    [Fact]
    public void CreateCollisionInfo_InvalidVectorDimension_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CreateCollisionInfoCommand(
            new[] { new Vector(0) },
            new[] { new Vector(0, 0) },
            new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 }
        ));
    }

    [Fact]
    public void CreateCollisionTreeCommand_ExecutesSuccessfully()
    {
        var states = new List<(int, int, int, int)> { (1, 2, 3, 4) };
        var cmd = new CreateCollisionTreeCommand(states);
        cmd.Execute();
        
        Assert.NotNull(cmd.Tree);
        Assert.True(cmd.Tree.Contains((1, 2, 3, 4)));
    }
}

