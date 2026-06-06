using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class SpaceTorpedoTests
{
    [Fact]
    public void ShouldStoreAndRetrieveState()
    {
        var torpedo = new SpaceTorpedo(new Dictionary<string, object>());
        var pos = new Vector(10, 20);
        torpedo.Position = pos;
        Assert.Equal(pos, torpedo.Position);
    }

    [Fact]
    public void ShouldBeCompatibleWithMoveCommand()
    {
        var torpedo = new SpaceTorpedo(new Dictionary<string, object>
        {
            ["Position"] = new Vector(0, 0),
            ["Velocity"] = new Vector(1, 1)
        });

        new MoveCommand(torpedo).Execute();
        Assert.Equal(new Vector(1, 1), torpedo.Position);
    }

    [Fact]
    public void ShouldThrowWhenDataMissing()
    {
        var torpedo = new SpaceTorpedo(new Dictionary<string, object>());
        Assert.Throws<KeyNotFoundException>(() => new MoveCommand(torpedo).Execute());
    }

    [Fact]
    public void GettersShouldReturnCorrectValues()
    {
        var torpedo = new SpaceTorpedo(new Dictionary<string, object>
        {
            ["Position"] = new Vector(1, 1),
            ["Velocity"] = new Vector(2, 2),
            ["Direction"] = new Angle(45)
        });

        Assert.Equal(new Vector(1, 1), torpedo.GetPosition());
        Assert.Equal(new Vector(2, 2), torpedo.GetVelocity());
        Assert.Equal(new Angle(45), torpedo.GetDirection());
    }
}

