using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class RotateCommandTests
{
    [Fact]
    public void Execute_ShouldRotate_WhenValidInput()
    {
        var mockRotating = new Mock<IRotating>();
        mockRotating.SetupProperty(m => m.Angle, new Angle(1));
        mockRotating.SetupGet(m => m.AngularVelocity).Returns(new Angle(1));
        
        var command = new RotateCommand(mockRotating.Object);
        command.Execute();

        Assert.Equal(new Angle(2), mockRotating.Object.Angle);
    }
    

    [Fact]
    public void Execute_ShouldThrowException_WhenAngleCannotBeRead()
    {
        var mockRotating = new Mock<IRotating>();
        mockRotating.SetupGet(m => m.Angle).Throws<Exception>();
        
        var command = new RotateCommand(mockRotating.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }


    [Fact]
    public void Execute_ShouldThrowException_WhenAngularVelocityCannotBeRead()
    {
        var mockRotating = new Mock<IRotating>();
        mockRotating.SetupGet(m => m.Angle).Returns(new Angle(1));
        mockRotating.SetupGet(m => m.AngularVelocity).Throws<Exception>();

        var command = new RotateCommand(mockRotating.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }


    [Fact]
    public void Execute_ShouldThrowException_WhenAngleCannotBeSet()
    {
        var mockRotating = new Mock<IRotating>();
        mockRotating.SetupGet(m => m.Angle).Returns(new Angle(1));
        mockRotating.SetupGet(m => m.AngularVelocity).Returns(new Angle(1));
        mockRotating.SetupSet(m => m.Angle = It.IsAny<Angle>()).Throws<Exception>();

        var command = new RotateCommand(mockRotating.Object);

        Assert.ThrowsAny<Exception>(() => command.Execute());
    }
}