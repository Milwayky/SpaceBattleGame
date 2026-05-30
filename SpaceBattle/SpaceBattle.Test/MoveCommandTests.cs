using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class MoveCommandTests
{
    [Fact]
    public void Execute_ShouldChangePosition_WhenValidInput()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupProperty(m => m.Position, new Vector(12, 5));
        mockMoving.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));
        
        var command = new MoveCommand(mockMoving.Object);
        command.Execute();
        Assert.Equal(new Vector(8, 6), mockMoving.Object.Position);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenPositionCannotBeRead()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupGet(m => m.Position).Throws<Exception>();
        mockMoving.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));

        var command = new MoveCommand(mockMoving.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenVelocityCannotBeRead()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupGet(m => m.Position).Returns(new Vector(12, 5));
        mockMoving.SetupGet(m => m.Velocity).Throws<Exception>();

        var command = new MoveCommand(mockMoving.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }


    [Fact]
    public void Execute_ShouldThrowException_WhenPositionCannotBeSet()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupGet(m => m.Position).Returns(new Vector(12, 5));
        mockMoving.SetupGet(m => m.Velocity).Returns(new Vector(-4, 1));
        
        mockMoving.SetupSet(m => m.Position = It.IsAny<Vector>()).Throws<Exception>();

        var command = new MoveCommand(mockMoving.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }
}  

