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

    [Fact]
    public void GameLoopConditionCommand_ShouldRegisterNewShouldContinueCondition()
    {
        // Регистрируем Game.Queue, возвращая мок
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue", (object[] args) => new Mock<IGameQueue>().Object).Execute();
        
        // КРИТИЧЕСКИЙ ФИКС: явно приводим 0 к object, чтобы IoC не падал с InvalidCastException
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.StartTime.Get", (object[] args) => (object)0).Execute();
        
        var mockSetTime = new Mock<ICommand>();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.StartTime.Set", (object[] args) => mockSetTime.Object).Execute();
        
        // Тут тоже кастуем к object на всякий случай
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Loop.CheckStatus", (object[] args) => (object)true).Execute();

        var conditionCommand = new GameLoopConditionCommand();
        conditionCommand.Execute();

        // Проверяем, что команда установки времени выполнилась
        mockSetTime.Verify(m => m.Execute(), Times.Once);

        // Проверяем, что перерегистрация Game.Loop.ShouldContinue прошла успешно и возвращает true
        var result = Ioc.Resolve<bool>("Game.Loop.ShouldContinue");
        Assert.True(result);
    }

    [Fact]
    public void RegisterSchedulerDependencies_ShouldRegisterQueueDependencies()
    {
        var mockQueue = new Mock<IGameQueue>();
        mockQueue.Setup(q => q.Count).Returns(5);
        var mockCmd = new Mock<ICommand>();
        mockQueue.Setup(q => q.Dequeue()).Returns(mockCmd.Object);

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue", (object[] args) => mockQueue.Object).Execute();

        // Выполняем регистратор зависимостей
        var registerCommand = new RegisterSchedulerDependencies();
        registerCommand.Execute();

        // Проверяем работу зарегистрированного ShouldContinue
        var shouldContinue = (bool)Ioc.Resolve<object>("Game.Loop.ShouldContinue");
        Assert.True(shouldContinue);

        // Проверяем работу зарегистрированного DequeueCommand
        var dequeuedCmd = Ioc.Resolve<ICommand>("Game.Loop.DequeueCommand");
        Assert.Same(mockCmd.Object, dequeuedCmd);
    }

    [Fact]
    public void UpdateGameCommand_ShouldExecuteGameUpdate()
    {
        var gameMock = new Mock<IGame>();
        var updateCommand = new UpdateGameCommand(gameMock.Object);

        updateCommand.Execute();

        gameMock.Verify(g => g.Update(), Times.Once);
    }
}

