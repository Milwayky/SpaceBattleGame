namespace SpaceBattle.Tests;
using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;
using System.Collections.Generic;

public class CreateMacroCommandStrategyTests
{
    public CreateMacroCommandStrategyTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        new RegisterIoCDependencyMacroCommand().Execute();
    }


    [Fact]
    public void TestMacroTestResolvesSuccessfully()
    {
        var obj = new Mock<object>().Object;
        Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Test", (object[] args) => new List<string>()).Execute();

        var strategy = new CreateMacroCommandStrategy("Specs.Test");
        var macro = strategy.Resolve(obj);

        Assert.NotNull(macro);
        Assert.IsType<MacroCommand>(macro);
    }

    [Fact]
    public void TestAllCommandsInMacroAreExecuted()
    {
        var obj = new Mock<object>().Object;
        var cmd1 = new Mock<SpaceBattle.Lib.ICommand>();
        var cmd2 = new Mock<SpaceBattle.Lib.ICommand>();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Test", (object[] args) => new List<string> { "C1", "C2" }).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "C1", (object[] args) => cmd1.Object).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "C2", (object[] args) => cmd2.Object).Execute();

        var strategy = new CreateMacroCommandStrategy("Specs.Test");
        var macro = strategy.Resolve(obj);
        macro.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }


    [Fact]
    public void TestResolveThrowsExceptionWhenSpecMissing()
    {
        var obj = new Mock<object>().Object;
        var strategy = new CreateMacroCommandStrategy("Specs.Missing");

        Assert.ThrowsAny<System.Exception>(() => strategy.Resolve(obj));
    }
}

