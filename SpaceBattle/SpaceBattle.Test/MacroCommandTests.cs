namespace SpaceBattle.Tests;

using SpaceBattle.Lib;
using Moq;
using Xunit;

public class MacroCommandTest
{
    [Fact]
    public void TestMacroCommandRunsAll()
    {
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        var cmd3 = new Mock<ICommand>();

        var macroCmd = new MacroCommand(new ICommand[] { cmd1.Object, cmd2.Object, cmd3.Object });

        macroCmd.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void TestMacroCommandThrowsExceptionWhenAnyCommandThrowsException()
    {
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        cmd2.Setup(c => c.Execute()).Throws<System.Exception>();
        var cmd3 = new Mock<ICommand>();

        var macroCmd = new MacroCommand(new ICommand[] { cmd1.Object, cmd2.Object, cmd3.Object });

        Assert.Throws<System.Exception>(() => macroCmd.Execute());

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Never());
    }

    [Fact]
    public void EmptyArrayDoesNotThrowException()
    {
        var macroCmd = new MacroCommand(new ICommand[] { });

        var exception = Record.Exception(() => macroCmd.Execute());

        Assert.Null(exception);
    }
}

