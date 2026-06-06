namespace SpaceBattle.Tests;

using App;
using App.Scopes;
using SpaceBattle.Lib;
using Moq;

public class RegisterIoCDependencyActionsStartTests
{
    public RegisterIoCDependencyActionsStartTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void RegisterIoCDependencyActionsStart_Should_Resolve_Dependency()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>
        {
            { "command", "TestCommand" },
            { "args", new object[] { } }
        };

        var result = Ioc.Resolve<App.ICommand>("Actions.Start", order);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<App.ICommand>(result);
    }


    [Fact]
    public void StartCommand_Should_Call_SendCommand()
    {
        var cmdMock = new Mock<Lib.ICommand>();

        Ioc.Resolve<App.ICommand>("IoC.Register",
            "Commands.TestCommand",
            (object[] args) => cmdMock.Object).Execute();

        var receiverMock = new Mock<ICommandReceiver>();

        Ioc.Resolve<App.ICommand>("IoC.Register",
            "Game.CommandsReceiver",
            (object[] args) => receiverMock.Object).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register",
            "Commands.CommandInjectable",
            (object[] args) => new CommandInjectableCommand()).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register",
            "Macro.TestCommand",
            (object[] args) => cmdMock.Object).Execute();

        var order = new Dictionary<string, object>
        {
            { "command", "TestCommand" },
            { "args", new object[] { } }
        };

        var cmd = new StartCommand(order);

        cmd.Execute();

        receiverMock.Verify(r => r.Receive(It.IsAny<Lib.ICommand>()), Times.AtLeastOnce);
    }
}

