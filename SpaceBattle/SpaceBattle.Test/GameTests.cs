using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib; // Вот этот using обязателен, чтобы тесты видели Game и IGame
using System;
using System.Collections.Generic;
using Xunit;

namespace SpaceBattle.Lib.Tests;

public class GameTests
{
    public GameTests()
    {
        // Инициализация IoC инфраструктуры для обработки ExceptionHandler внутри тестов
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void Game_Update_ShouldIncrementTickAndSetLastUpdated()
    {
        var state = new Dictionary<string, object>();
        var game = new Game(state);

        game.Update();

        Assert.Equal(1, state["Tick"]);
        Assert.True(state.ContainsKey("LastUpdated"));
    }

    [Fact]
    public void Game_Update_ShouldExecuteGameEvents()
    {
        var mockCmd = new Mock<ICommand>();
        var eventsQueue = new Queue<ICommand>();
        eventsQueue.Enqueue(mockCmd.Object);

        var state = new Dictionary<string, object>
        {
            ["GameEvents"] = eventsQueue
        };
        var game = new Game(state);

        game.Update();

        mockCmd.Verify(c => c.Execute(), Times.Once);
        Assert.Empty(eventsQueue);
    }

    [Fact]
    public void Game_Update_ShouldHandleExceptionsInGameEvents()
    {
        var failingCmd = new Mock<ICommand>();
        var exception = new Exception("Event failed");
        failingCmd.Setup(c => c.Execute()).Throws(exception);

        var eventsQueue = new Queue<ICommand>();
        eventsQueue.Enqueue(failingCmd.Object);

        var handlerMock = new Mock<ICommand>();
        Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler.Handle", 
            (object[] args) => handlerMock.Object).Execute();

        var state = new Dictionary<string, object>
        {
            ["GameEvents"] = eventsQueue
        };
        var game = new Game(state);

        game.Update();

        handlerMock.Verify(h => h.Execute(), Times.Once);
    }

    [Fact]
    public void Game_Update_ShouldSetIsGameOver_WhenMaxTicksReached()
    {
        var state = new Dictionary<string, object>
        {
            ["Tick"] = 9,
            ["MaxTicks"] = 10
        };
        var game = new Game(state);

        game.Update();

        Assert.Equal(10, state["Tick"]);
        Assert.True((bool)state["IsGameOver"]);
    }
}
