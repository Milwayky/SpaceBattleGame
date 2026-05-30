namespace SpaceBattle.Tests;
using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;

public class RegisterIoCDependencySendCommandTest
{
    public RegisterIoCDependencySendCommandTest()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void TestExecuteRegisterIoCDependencySendCommandResolvesDependency()
    {
        var mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();
        
        var registerCommand = new RegisterIoCDependencySendCommand();

        registerCommand.Execute();

        var result = Ioc.Resolve<SpaceBattle.Lib.ICommand>(
            "Commands.Send", 
            mockCommand.Object, 
            mockReceiver.Object
        );

        Assert.NotNull(result);
        Assert.IsType<SendCommand>(result);
    }
}

