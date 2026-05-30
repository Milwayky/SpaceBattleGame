namespace SpaceBattle.Tests;
using App;
using App.Scopes;
using SpaceBattle.Lib;
using Moq;
using Xunit;

public class RegisterDependencyCommandInjectableCommandTest
{
    public RegisterDependencyCommandInjectableCommandTest()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }


    [Fact]
    public void RegisterDependencyResolvedICommandType()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();
        var cmd = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Commands.CommandInjectable");

        Assert.IsAssignableFrom<SpaceBattle.Lib.ICommand>(cmd);
    }

    [Fact]
    public void RegisterDependencyResolvedICommandInjectableType()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();
        var cmd = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");

        Assert.IsAssignableFrom<ICommandInjectable>(cmd);
    }

    [Fact]
    public void RegisterDependencyResolvedICommandInjectableCommandType()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();
        var cmd = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.IsType<CommandInjectableCommand>(cmd);
    }
}

