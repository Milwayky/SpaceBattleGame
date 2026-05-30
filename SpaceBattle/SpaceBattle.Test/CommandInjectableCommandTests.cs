namespace SpaceBattle.Tests;
using Moq;
using SpaceBattle.Lib;
using Xunit;

public class CommandInjectableCommandTests
{
    [Fact]
    public void TestCommandInjectable_ExecutesInjectedCommand()
    {
        var injectable = new CommandInjectableCommand();
        var mockCommand = new Mock<ICommand>();
        
        injectable.Inject(mockCommand.Object);
        injectable.Execute();

        mockCommand.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void TestCommandInjectable_ThrowsExceptionIfNoCommandInjected()
    {
        var injectable = new CommandInjectableCommand();

        Assert.ThrowsAny<System.Exception>(() => injectable.Execute());
    }
}

