using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;
using System.Collections.Generic;

namespace SpaceBattle.Lib.Tests;

// Отключаем параллельный запуск для этого класса, чтобы скопы не дрались
[Collection("SequentialTests")]
public class GameLoopCommandTests
{
    public GameLoopCommandTests()
    {
        new InitCommand().Execute();
        var scope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();
    }

    [Fact]
    public void GameLoopCommand_ProcessCommand_WhenSequenceAllows()
    {
        var context = Ioc.Resolve<object>("IoC.Scope.Create");
        var loopCommand = new GameLoopCommand(context);

        var taskMock = new Mock<ICommand>();
        var signals = new Queue<bool>(new[] { true, false });

        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed",
            (object[] args) => (object)signals.Dequeue()).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.FetchNext",
            (object[] args) => taskMock.Object).Execute();

        loopCommand.Execute();

        taskMock.Verify(t => t.Execute(), Times.Once);
    }

    [Fact]
    public void GameLoopCommand_ShouldRedirectToErrorHandler_OnException()
    {
        var context = Ioc.Resolve<object>("IoC.Scope.Create");
        var loopCommand = new GameLoopCommand(context);
        
        var brokenTask = new Mock<ICommand>();
        var testException = new Exception("Engine crash simulation");
        brokenTask.Setup(t => t.Execute()).Throws(testException);

        var diagnosticHandler = new Mock<ICommand>();
        var signals = new Queue<bool>(new[] { true, false });

        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed", (object[] args) => (object)signals.Dequeue()).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.FetchNext", (object[] args) => brokenTask.Object).Execute();
        
        // ИСПРАВЛЕНО: Регистрация обработчика ошибок должна идти через "IoC.Register"
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Errors.Handle", (object[] args) => diagnosticHandler.Object).Execute();

        loopCommand.Execute();

        diagnosticHandler.Verify(h => h.Execute(), Times.Once);
    }
}

