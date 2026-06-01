using Moq;
using SpaceBattle.Lib;
using Xunit;

public class FireCommandTests
{
    [Fact]
    public void Fire_InitializesBulletWithShooterParams()
    {
        var pos = new Vector([1, 1]);
        var vel = new Vector([2, 2]);
        var dir = new Angle(90);

        var shooter = new Mock<IShootable>();
        shooter.Setup(s => s.GetPosition()).Returns(pos);
        shooter.Setup(s => s.GetVelocity()).Returns(vel);
        shooter.Setup(s => s.GetDirection()).Returns(dir);

        var bullet = new Mock<IWeaponized>();
        var command = new FireCommand(shooter.Object, bullet.Object);

        command.Execute();

        bullet.Verify(b => b.Initialize(pos, vel, dir), Times.Once);
    }

    [Fact]
    public void Fire_ThrowsException_WhenShooterCannotProvidePosition()
    {
        var shooter = new Mock<IShootable>();
        var bullet = new Mock<IWeaponized>();
        
        shooter.Setup(s => s.GetPosition()).Throws(new Exception());
        var command = new FireCommand(shooter.Object, bullet.Object);

        Assert.Throws<Exception>(() => command.Execute());
        bullet.Verify(b => b.Initialize(It.IsAny<Vector>(), It.IsAny<Vector>(), It.IsAny<Angle>()), Times.Never);
    }

    [Fact]
    public void Fire_ThrowsException_WhenBulletCannotBeInitialized()
    {
        var shooter = new Mock<IShootable>();
        var bullet = new Mock<IWeaponized>();
        
        bullet.Setup(b => b.Initialize(It.IsAny<Vector>(), It.IsAny<Vector>(), It.IsAny<Angle>()))
            .Throws(new Exception());
        
        var command = new FireCommand(shooter.Object, bullet.Object);
        Assert.Throws<Exception>(() => command.Execute());
    }
}

