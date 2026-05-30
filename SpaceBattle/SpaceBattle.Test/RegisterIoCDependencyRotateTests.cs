namespace SpaceBattle.Tests;

using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;

public class RegisterIoCDependencyRotateCommandTest
{
    public RegisterIoCDependencyRotateCommandTest()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void TestExecuteRegisterIoCDependencyRotateCommandResolvesDependency()
    {
        var obj = new Mock<object>().Object;
        var rotatingAdapterMock = new Mock<IRotating>();

        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Adapters.IRotating",
            (object[] args) => rotatingAdapterMock.Object
        ).Execute();

        var registerIoCDependencyRotateCommand = new RegisterIoCDependencyRotateCommand();

        registerIoCDependencyRotateCommand.Execute();

        var rotateCmd = Ioc.Resolve<Lib.ICommand>("Commands.Rotate", obj);

        Assert.IsType<RotateCommand>(rotateCmd);
    }
}

