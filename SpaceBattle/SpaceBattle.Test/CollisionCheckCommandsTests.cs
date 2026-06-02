using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionCheckCommandsTests
{
    private static CollisionObject CreateObj(string form, int[] pos, int[] vel)
    {
        return new CollisionObject(new Dictionary<string, object>
        {
            ["Form"] = form,
            ["Position"] = new Vector(pos),
            ["Velocity"] = new Vector(vel)
        });
    }

    [Fact]
    public void CheckCollision_DirectTreeHit_SetsHasCollisionTrue()
    {
        var storage = new Dictionary<(string, string), CollisionTree>
        {
            [("ship", "torpedo")] = new CollisionTree(new[] { (1, 1, 1, 1) })
        };
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 0, 0 });
        var torpedo = CreateObj("torpedo", new[] { 1, 1 }, new[] { 1, 1 });

        var cmd = new CheckCollisionCommand(ship, torpedo, storage);
        cmd.Execute();

        Assert.True(cmd.HasCollision);
    }

    [Fact]
    public void CheckCollision_ReverseTreeHit_SetsHasCollisionTrue()
    {
        var storage = new Dictionary<(string, string), CollisionTree>
        {
            [("torpedo", "ship")] = new CollisionTree(new[] { (-1, -1, -1, -1) })
        };
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 0, 0 });
        var torpedo = CreateObj("torpedo", new[] { 1, 1 }, new[] { 1, 1 });

        var cmd = new CheckCollisionCommand(ship, torpedo, storage);
        cmd.Execute();

        Assert.True(cmd.HasCollision);
    }

    [Fact]
    public void CheckCollision_NoTreeExists_SetsHasCollisionFalse()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 0, 0 });
        var torpedo = CreateObj("torpedo", new[] { 1, 1 }, new[] { 1, 1 });

        var cmd = new CheckCollisionCommand(ship, torpedo, storage);
        cmd.Execute();

        Assert.False(cmd.HasCollision);
    }

    [Fact]
    public void CheckCollision_InvalidVectorDimension_ThrowsArgumentException()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        var brokenShip = CreateObj("ship", new[] { 1 }, new[] { 0, 0 }); // 1D вектор
        var torpedo = CreateObj("torpedo", new[] { 1, 1 }, new[] { 1, 1 });

        var cmd = new CheckCollisionCommand(brokenShip, torpedo, storage);

        Assert.Throws<ArgumentException>(() => cmd.Execute());
    }

    [Fact]
    public void CheckCollisions_FiltersOutTargetItself_AndReturnsCollided()
    {
        var storage = new Dictionary<(string, string), CollisionTree>
        {
            [("ship", "torpedo")] = new CollisionTree(new[] { (1, 0, 0, 0) })
        };
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 1, 0 });
        var targetTorpedo = CreateObj("torpedo", new[] { 1, 0 }, new[] { 1, 0 });

        var cmd = new CheckCollisionsCommand(ship, new[] { ship, targetTorpedo }, storage);
        cmd.Execute();

        Assert.Single(cmd.CollidedObjects);
        Assert.Same(targetTorpedo, cmd.CollidedObjects.First());
    }

    [Fact]
    public void MoveWithCollisionCheckCommand_ExecutesBothInternalCommands()
    {
        var mockMove = new Mock<SpaceBattle.Lib.ICommand>();
        var storage = new Dictionary<(string, string), CollisionTree>();
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 0, 0 });
        
        var checkCmd = new CheckCollisionsCommand(ship, Enumerable.Empty<ICollisionObject>(), storage);
        var macroCmd = new MoveWithCollisionCheckCommand(mockMove.Object, checkCmd);

        macroCmd.Execute();

        mockMove.Verify(m => m.Execute(), Times.Once);
        Assert.Empty(macroCmd.CollidedObjects);
    }

    [Fact]
    public void CheckCollisions_EmptyStorage_ReturnsEmptyList()
    {
        var ship = CreateObj("ship", new[] { 0, 0 }, new[] { 1, 0 });
        var cmd = new CheckCollisionsCommand(ship, new[] { ship }, new Dictionary<(string, string), CollisionTree>());
        cmd.Execute();
        Assert.Empty(cmd.CollidedObjects);
    }

    [Fact]
    public void CheckCollision_WithNullObjects_ThrowsException()
    {
        var storage = new Dictionary<(string, string), CollisionTree>();
        Assert.Throws<NullReferenceException>(() => new CheckCollisionCommand(null!, null!, storage).Execute());
    }

}

