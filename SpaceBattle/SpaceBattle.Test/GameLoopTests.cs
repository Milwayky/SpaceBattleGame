using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Lib.Tests;

public class GameLoopTests
{
    public GameLoopTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void GameLoop_ShouldExecuteCommandFromQueue_AndTerminate()
    {
        var gameScope = Ioc.Resolve<object>("IoC.Scope.Create");
        var loop = new GameLoopCommand(gameScope);

        var commandMock = new Mock<ICommand>();
        
        // Условие: один раз true, затем false (выход из цикла)
        var continueStates = new Queue<bool>(new[] { true, false });

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.ShouldContinue",
            (object[] args) => (object)continueStates.Dequeue()).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.DequeueCommand",
            (object[] args) => commandMock.Object).Execute();

        loop.Execute();

        commandMock.Verify(m => m.Execute(), Times.Once);
    }

    [Fact]
    public void GameLoop_ShouldHandleException_ViaExceptionHandler()
    {
        var gameScope = Ioc.Resolve<object>("IoC.Scope.Create");
        var loop = new GameLoopCommand(gameScope);
        
        var failingCommand = new Mock<ICommand>();
        var exception = new Exception("Loop fail");
        failingCommand.Setup(c => c.Execute()).Throws(exception);

        var handlerMock = new Mock<ICommand>();
        var continueStates = new Queue<bool>(new[] { true, false });

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.ShouldContinue", 
            (object[] args) => (object)continueStates.Dequeue()).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.DequeueCommand", 
            (object[] args) => failingCommand.Object).Execute();

        // Регистрируем хендлер, который возвращает мок-команду обработки исключения
        Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] args) => 
            (object)handlerMock.Object).Execute();

        loop.Execute();

        handlerMock.Verify(h => h.Execute(), Times.Once);
    }
}

