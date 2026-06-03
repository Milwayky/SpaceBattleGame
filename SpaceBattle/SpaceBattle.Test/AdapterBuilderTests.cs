using SpaceBattle.Lib;
using Xunit;
using System;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public interface ITestAdapter
{
    string Name { get; set; }
    int Value { get; set; }
    Vector Position { get; set; }
}

public class AdapterBuilderTests
{
    [Fact]
    public void Build_ShouldCreateAdapter_WithCorrectPropertyValues()
    {
        var data = new Dictionary<string, object>
        {
            ["Name"] = "TestObject",
            ["Value"] = 42,
            ["Position"] = new Vector(10, 20)
        };

        var adapter = AdapterBuilder.Build<ITestAdapter>(data);

        Assert.Equal("TestObject", adapter.Name);
        Assert.Equal(42, adapter.Value);
        Assert.Equal(new Vector(10, 20), adapter.Position);
    }

    [Fact]
    public void Build_ShouldAllowPropertyModification()
    {
        var data = new Dictionary<string, object> { ["Name"] = "OldName" };
        var adapter = AdapterBuilder.Build<ITestAdapter>(data);

        adapter.Name = "NewName";

        Assert.Equal("NewName", data["Name"]);
        Assert.Equal("NewName", adapter.Name);
    }

    [Fact]
    public void Build_ShouldThrowKeyNotFoundException_WhenKeyMissing()
    {
        var data = new Dictionary<string, object>();
        var adapter = AdapterBuilder.Build<ITestAdapter>(data);

        Assert.Throws<KeyNotFoundException>(() => adapter.Name);
    }

    [Fact]
    public void Build_ShouldCacheAdapterTypes()
    {
        var data1 = new Dictionary<string, object> { ["Name"] = "First" };
        var data2 = new Dictionary<string, object> { ["Name"] = "Second" };

        var adapter1 = AdapterBuilder.Build<ITestAdapter>(data1);
        var adapter2 = AdapterBuilder.Build<ITestAdapter>(data2);

        Assert.Same(adapter1.GetType(), adapter2.GetType());
        Assert.Equal("First", adapter1.Name);
        Assert.Equal("Second", adapter2.Name);
    }

    [Fact]
    public void Build_IMoving_Adapter_WorksCorrectly()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(12, 5),
            ["Velocity"] = new Vector(-4, 1)
        };

        var adapter = AdapterBuilder.Build<IMoving>(data);

        Assert.Equal(new Vector(12, 5), adapter.Position);
        Assert.Equal(new Vector(-4, 1), adapter.Velocity);

        adapter.Position = new Vector(100, 200);
        Assert.Equal(new Vector(100, 200), data["Position"]);
    }

    [Fact]
    public void Build_IRotating_Adapter_WorksCorrectly()
    {
        var data = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(1),
            ["AngularVelocity"] = new Angle(2)
        };

        var adapter = AdapterBuilder.Build<IRotating>(data);

        Assert.Equal(new Angle(1), adapter.Angle);
        Assert.Equal(new Angle(2), adapter.AngularVelocity);
    }

    [Fact]
    public void Build_ShouldThrow_ForNonInterfaceType()
    {
        Assert.Throws<ArgumentException>(() =>
            AdapterBuilder.Build(typeof(string), new Dictionary<string, object>()));
    }

    [Fact]
    public void Build_MoveCommand_FromAdaptedDictionary_ChangesPosition()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(12, 5),
            ["Velocity"] = new Vector(-4, 1)
        };

        var adapter = AdapterBuilder.Build<IMoving>(data);
        var moveCmd = new MoveCommand(adapter);
        moveCmd.Execute();

        Assert.Equal(new Vector(8, 6), adapter.Position);
    }

    [Fact]
    public void Build_RotateCommand_FromAdaptedDictionary_ChangesAngle()
    {
        var data = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(1),
            ["AngularVelocity"] = new Angle(1)
        };

        var adapter = AdapterBuilder.Build<IRotating>(data);
        var rotateCmd = new RotateCommand(adapter);
        rotateCmd.Execute();

        Assert.Equal(new Angle(2), adapter.Angle);
    }
}