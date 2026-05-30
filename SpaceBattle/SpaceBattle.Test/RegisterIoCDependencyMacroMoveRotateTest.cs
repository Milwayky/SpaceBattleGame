namespace SpaceBattle.Tests;

using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;
using System.Collections.Generic;

public class RegisterIoCDependencyMacroMoveRotateTest
{
    public RegisterIoCDependencyMacroMoveRotateTest()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        new RegisterIoCDependencyMacroCommand().Execute();
    }


    [Fact]
    public void TestMacroMoveResolvesAndExecutes()
    {
        var obj = new Mock<object>().Object;
        var moveCmdMock = new Mock<SpaceBattle.Lib.ICommand>();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Move", (object[] args) => 
            new List<string> { "Commands.Move" }).Execute();
        
        Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Move", (object[] args) => 
            moveCmdMock.Object).Execute();

        var regCommand = new RegisterIoCDependencyMacroMoveRotate();
        regCommand.Execute();
        var macro = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Macro.Move", obj);
        macro.Execute();

        Assert.NotNull(macro);
        moveCmdMock.Verify(m => m.Execute(), Times.Once());
    }


    [Fact]
    public void TestMacroRotateResolvesAndExecutes()
    {
        var obj = new Mock<object>().Object;
        var rotateCmdMock = new Mock<SpaceBattle.Lib.ICommand>();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Rotate", (object[] args) => 
            new List<string> { "Commands.Rotate" }).Execute();
        
        Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Rotate", (object[] args) => 
            rotateCmdMock.Object).Execute();

        var regCommand = new RegisterIoCDependencyMacroMoveRotate();

        regCommand.Execute();
        var macro = Ioc.Resolve<SpaceBattle.Lib.ICommand>("Macro.Rotate", obj);
        macro.Execute();

        Assert.NotNull(macro);
        rotateCmdMock.Verify(m => m.Execute(), Times.Once());
    }
}
