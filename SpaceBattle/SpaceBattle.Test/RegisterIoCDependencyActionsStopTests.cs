namespace SpaceBattle.Tests;

using App;
using App.Scopes;
using SpaceBattle.Lib;
using Moq;

public class RegisterIoCDependencyActionsStopTests
{
    public RegisterIoCDependencyActionsStopTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void RegisterIoCDependencyActionsStop_Should_Resolve_Dependency()
    {
        new RegisterIoCDependencyActionsStop().Execute();

        var order = new Dictionary<string, object>
        {
            { "command", "TestCommand" }
        };

        var result = Ioc.Resolve<App.ICommand>("Actions.Stop", order);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<App.ICommand>(result);
    }

    [Fact]
    public void StopCommand_Should_Inject_EmptyCommand_And_Remove_From_Order()
    {
        var injectableMock = new Mock<ICommandInjectable>();

        var order = new Dictionary<string, object>
        {
            { "command", "TestCommand" },
            { "repeatableTestCommand", injectableMock.Object }
        };

        var stopCmd = new StopCommand(order);

        stopCmd.Execute();

        injectableMock.Verify(
            i => i.Inject(It.IsAny<Lib.ICommand>()),
            Times.Once
        );

        Assert.False(order.ContainsKey("repeatableTestCommand"));
    }

    [Fact]
    public void StopCommand_Should_Throw_When_Command_Not_Started()
    {
        var order = new Dictionary<string, object>
        {
            { "command", "TestCommand" }
        };

        var stopCmd = new StopCommand(order);

        Assert.Throws<InvalidOperationException>(() => stopCmd.Execute());
    }
}

