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
    public void Properties_SupportAlternativeRepositoryKeys()
    {
        var gameObject = new Dictionary<string, object>
        {
            ["Type"] = "torpedo",
            ["Location"] = new Vector(5, 6),
            ["Velocity"] = new Vector(7, 8)
        };

        var collisionObject = new CollisionObject(gameObject);

        Assert.Equal("torpedo", collisionObject.Form);
        Assert.Equal(new Vector(5, 6), collisionObject.Position);
        Assert.Equal(new Vector(7, 8), collisionObject.Velocity);
    }

    [Fact]
    public void MissingProperty_ThrowsKeyNotFoundException()
    {
        var collisionObject = new CollisionObject(new Dictionary<string, object>());

        Assert.Throws<KeyNotFoundException>(() => collisionObject.Form);
        Assert.Throws<KeyNotFoundException>(() => collisionObject.Position);
        Assert.Throws<KeyNotFoundException>(() => collisionObject.Velocity);
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

        Assert.Equal("ship", collisionObject.Form);
        Assert.Equal(new Vector(1, 2), collisionObject.Position);
        Assert.Throws<KeyNotFoundException>(() => collisionObject.Velocity);
    }

    [Fact]
    public void GetValue_ReturnsCorrectValue_WhenKeyExists()
    {
        var gameObject = new Dictionary<string, object> { ["Type"] = "torpedo" };
        var collisionObject = new CollisionObject(gameObject);
        
        Assert.Equal("torpedo", collisionObject.Form);
    }
}

