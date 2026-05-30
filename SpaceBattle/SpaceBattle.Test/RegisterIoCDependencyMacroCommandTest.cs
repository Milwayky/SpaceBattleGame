namespace SpaceBattle.Tests;

using App;
using Moq;
using App.Scopes;

using SpaceBattle.Lib;
using Xunit;
using System.Collections.Generic;

public class RegisterIoCDependencyMacroCommandTest
{
    public RegisterIoCDependencyMacroCommandTest()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void TestExecuteRegisterIoCDependencyMacroCommandResolvesDependency()
    {
        var registerCommand = new RegisterIoCDependencyMacroCommand();
        var mockCmd1 = new Mock<SpaceBattle.Lib.ICommand>();
        var mockCmd2 = new Mock<SpaceBattle.Lib.ICommand>();
        var commandList = new List<SpaceBattle.Lib.ICommand> { mockCmd1.Object, mockCmd2.Object };

        registerCommand.Execute();

        var macroCmd = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Commands.Macro", commandList);

        Assert.NotNull(macroCmd);
        Assert.IsType<MacroCommand>(macroCmd);
    }
}

