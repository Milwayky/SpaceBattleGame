using App;
using App.Scopes;
using SpaceBattle.Lib;
using Xunit;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyAutoAdapterTests
{
    // Используем статический конструктор, чтобы InitCommand 
    // выполнился только один раз при первом обращении к классу
    static RegisterIoCDependencyAutoAdapterTests()
    {
        new InitCommand().Execute();
    }

    public RegisterIoCDependencyAutoAdapterTests()
    {
        // Создаем новый scope для каждого теста, чтобы тесты были изолированы
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void RegisterAutoAdapter_ShouldResolveIMoving_FromDictionary()
    {
        new RegisterIoCDependencyAutoAdapter().Execute();

        var gameObject = new Dictionary<string, object>
        {
            ["Position"] = new Vector(12, 5),
            ["Velocity"] = new Vector(-4, 1)
        };

        var movingObj = Ioc.Resolve<IMoving>("Adapters.IMoving", gameObject);

        Assert.NotNull(movingObj);
        Assert.Equal(new Vector(12, 5), movingObj.Position);
        Assert.Equal(new Vector(-4, 1), movingObj.Velocity);
    }

    [Fact]
    public void RegisterAutoAdapter_ShouldResolveIRotating_FromDictionary()
    {
        new RegisterIoCDependencyAutoAdapter().Execute();

        var gameObject = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(1),
            ["AngularVelocity"] = new Angle(2)
        };

        var rotatingObj = Ioc.Resolve<IRotating>("Adapters.IRotating", gameObject);

        Assert.NotNull(rotatingObj);
        Assert.Equal(new Angle(1), rotatingObj.Angle);
        Assert.Equal(new Angle(2), rotatingObj.AngularVelocity);
    }

    [Fact]
    public void RegisterAutoMoveCommand_ShouldResolveAndExecuteCorrectly()
    {
        new RegisterIoCDependencyAutoAdapter().Execute();
        new RegisterIoCDependencyMoveCommandAuto().Execute();

        var gameObject = new Dictionary<string, object>
        {
            ["Position"] = new Vector(12, 5),
            ["Velocity"] = new Vector(-4, 1)
        };

        var moveCmd = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Commands.Move", gameObject);

        Assert.NotNull(moveCmd);
        Assert.IsType<MoveCommand>(moveCmd);

        moveCmd.Execute();

        Assert.Equal(new Vector(8, 6), (Vector)gameObject["Position"]);
    }

    [Fact]
    public void RegisterAutoRotateCommand_ShouldResolveAndExecuteCorrectly()
    {
        new RegisterIoCDependencyAutoAdapter().Execute();
        new RegisterIoCDependencyRotateCommandAuto().Execute();

        var gameObject = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(1),
            ["AngularVelocity"] = new Angle(1)
        };

        var rotateCmd = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Commands.Rotate", gameObject);

        Assert.NotNull(rotateCmd);
        Assert.IsType<RotateCommand>(rotateCmd);

        rotateCmd.Execute();

        Assert.Equal(new Angle(2), (Angle)gameObject["Angle"]);
    }
}