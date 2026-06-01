using Moq;
using SpaceBattle.Lib;
using Xunit;

public class PhotonTorpedoTests
{
    [Fact]
    public void ShouldCorrectlyStoreAndRetrieveState()
    {
        var storage = new Dictionary<string, object>();
        var torpedo = new SpaceTorpedo(storage);
        var pos = new Vector([10, 20]);

        torpedo.Position = pos;

        Assert.Equal(pos, torpedo.Position);
    }

    [Fact]
    public void ShouldBeCompatibleWithMoveCommand()
    {
        var data = new Dictionary<string, object> {
            ["Position"] = new Vector([0, 0]),
            ["Velocity"] = new Vector([1, 1])
        };
        var torpedo = new SpaceTorpedo(data);
        var move = new MoveCommand(torpedo);

        move.Execute();

        Assert.Equal(new Vector([1, 1]), torpedo.Position);
    }

    [Fact]
    public void ShouldThrowExceptionIfDataIsMissing()
    {
        var emptyData = new Dictionary<string, object>();
        var torpedo = new SpaceTorpedo(emptyData);
        var move = new MoveCommand(torpedo);

        Assert.Throws<KeyNotFoundException>(() => move.Execute());
    }

    [Fact]
    public void ShouldThrowWhenStorageAccessFails()
    {
        var mockStorage = new Mock<IDictionary<string, object>>();
        mockStorage.SetupGet(s => s[It.IsAny<string>()]).Throws(new InvalidOperationException());
        
        var torpedo = new SpaceTorpedo(mockStorage.Object);

        Assert.Throws<InvalidOperationException>(() => { var p = torpedo.Position; });
    }

    [Fact]
    public void ShouldCorrectlyStoreAndRetrieveVelocity()
    {
        var storage = new Dictionary<string, object>();
        var torpedo = new SpaceTorpedo(storage);
        var vel = new Vector([5, 5]);

        torpedo.Velocity = vel;

        Assert.Equal(vel, torpedo.Velocity);
    }

    [Fact]
    public void ShouldThrowWhenVelocityAccessFails()
    {
        var mockStorage = new Mock<IDictionary<string, object>>();
        mockStorage.SetupGet(s => s[It.IsAny<string>()]).Throws(new InvalidOperationException());
        
        var torpedo = new SpaceTorpedo(mockStorage.Object);

        Assert.Throws<InvalidOperationException>(() => { var v = torpedo.Velocity; });
    }

    [Fact]
    public void ShouldThrowWhenSettingVelocityFails()
    {
        var mockStorage = new Mock<IDictionary<string, object>>();
        
        mockStorage.SetupSet(s => s[It.IsAny<string>()] = It.IsAny<object>())
                .Throws(new InvalidOperationException());
        
        var torpedo = new SpaceTorpedo(mockStorage.Object);

        Assert.Throws<InvalidOperationException>(() => torpedo.Velocity = new Vector([1, 1]));
    }

    [Fact]
    public void ShouldCorrectlyStoreAndRetrieveDirection()
    {
        var storage = new Dictionary<string, object>();
        var torpedo = new SpaceTorpedo(storage);
        var angle = new Angle(90);

        torpedo.Direction = angle;

        Assert.Equal(angle, torpedo.Direction);
    }

    [Fact]
    public void GettersShouldReturnCorrectValues()
    {
        var data = new Dictionary<string, object> {
            ["Position"] = new Vector([1, 1]),
            ["Velocity"] = new Vector([2, 2]),
            ["Direction"] = new Angle(45)
        };
        var torpedo = new SpaceTorpedo(data);

        Assert.Equal(torpedo.Position, torpedo.GetPosition());
        Assert.Equal(torpedo.Velocity, torpedo.GetVelocity());
        Assert.Equal(torpedo.Direction, torpedo.GetDirection());
    }
}

