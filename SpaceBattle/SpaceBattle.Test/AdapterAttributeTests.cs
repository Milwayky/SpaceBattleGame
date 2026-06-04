using SpaceBattle.Lib;
using Xunit;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpaceBattle.Tests;

public static class ExampleVelocityStrategy
{
    public static object GetVelocity(IDictionary<string, object> data)
    {
        if (data.TryGetValue("Velocity", out var value) && value is Vector v)
            return v;
        throw new KeyNotFoundException("Velocity not found");
    }

    public static void SetVelocity(IDictionary<string, object> data, object value)
    {
        data["Velocity"] = value;
    }
}

public interface IMovingWithCustomVelocity
{
    Vector Position { get; set; }

    [Adapter(typeof(IMovingWithCustomVelocity), "Velocity",
        typeof(ExampleVelocityStrategy), nameof(ExampleVelocityStrategy.GetVelocity))]
    Vector Velocity { get; }
}

public class AdapterAttributeTests
{
    [Fact]
    public void AdapterAttribute_ShouldBeRetrievable_FromProperty()
    {
        var property = typeof(IMovingWithCustomVelocity).GetProperty("Velocity");
        Assert.NotNull(property);

        var attr = property!.GetCustomAttribute<AdapterAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("Velocity", attr!.PropertyName);
        Assert.Equal(typeof(IMovingWithCustomVelocity), attr.InterfaceType);
        Assert.Equal(nameof(ExampleVelocityStrategy.GetVelocity), attr.MethodName);
    }

    [Fact]
    public void AdapterAttribute_Constructor_ShouldSetAllProperties()
    {
        var attr = new AdapterAttribute(
            typeof(IMoving),
            "Velocity",
            typeof(ExampleVelocityStrategy),
            "GetVelocity"
        );

        Assert.Equal(typeof(IMoving), attr.InterfaceType);
        Assert.Equal("Velocity", attr.PropertyName);
        Assert.Equal(typeof(ExampleVelocityStrategy), attr.StrategyType);
        Assert.Equal("GetVelocity", attr.MethodName);
    }

    [Fact]
    public void AdapterBuilder_WithAdapterAttribute_ShouldCallCustomStrategy()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(1, 2),
            ["Velocity"] = new Vector(3, 4)
        };

        var adapter = AdapterBuilder.Build<IMovingWithCustomVelocity>(data);

        var velocity = adapter.Velocity;
        Assert.Equal(new Vector(3, 4), velocity);
    }

    [Fact]
    public void AdapterBuilder_MultipleProperties_WithAndWithoutAttributes()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(50, 60),
            ["Velocity"] = new Vector(7, 8)
        };

        var adapter = AdapterBuilder.Build<IMovingWithCustomVelocity>(data);

        Assert.Equal(new Vector(50, 60), adapter.Position);
        Assert.Equal(new Vector(7, 8), adapter.Velocity);
    }
}