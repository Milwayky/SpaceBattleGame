using System.Collections.Generic;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CollisionObjectTests
{
    [Fact]
    public void Properties_ReturnValuesFromDictionary_Successfully()
    {
        var gameObject = new Dictionary<string, object>
        {
            ["Form"] = "ship",
            ["Position"] = new Vector(1, 2),
            ["Velocity"] = new Vector(3, 4)
        };

        var collisionObject = new CollisionObject(gameObject);

        Assert.Equal("ship", collisionObject.Form);
        Assert.Equal(new Vector(1, 2), collisionObject.Position);
        Assert.Equal(new Vector(3, 4), collisionObject.Velocity);
    }

    [Fact]
    public void MissingProperty_ThrowsKeyNotFoundException()
    {
        var collisionObject = new CollisionObject(new Dictionary<string, object>());
        Assert.Throws<KeyNotFoundException>(() => collisionObject.Form);
    }

    [Fact]
    public void PartialMissingProperty_ThrowsKeyNotFoundException_ForVelocity()
    {
        var gameObject = new Dictionary<string, object>
        {
            ["Form"] = "ship",
            ["Position"] = new Vector(1, 2)
        };
        var collisionObject = new CollisionObject(gameObject);
        Assert.Throws<KeyNotFoundException>(() => collisionObject.Velocity);
    }

    [Fact]
    public void AlternativeKeys_ResolveSuccessfully()
    {
        var gameObject = new Dictionary<string, object> { ["Type"] = "ship" };
        var collisionObject = new CollisionObject(gameObject);
        Assert.Equal("ship", collisionObject.Form);
    }
}

