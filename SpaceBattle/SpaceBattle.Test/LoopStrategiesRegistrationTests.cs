using App;
using App.Scopes;
using Moq;
using Xunit;
using SpaceBattle.Lib;
using System.Collections.Generic;

namespace SpaceBattle.Lib.Tests;

// Склеиваем в одну коллекцию с первыми тестами, чтобы они шли строго друг за другом
[Collection("SequentialTests")]
public class LoopStrategiesRegistrationTests
{
    public LoopStrategiesRegistrationTests()
    {
        new InitCommand().Execute();
        var scope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();
    }

    [Fact]
    public void StandardStrategy_ShouldCheckContainerSize()
    {
        var containerMock = new Mock<ICommandContainer>();
        containerMock.SetupGet(c => c.Size).Returns(10);
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.Container", (object[] args) => (object)containerMock.Object).Execute();

        new RegisterLoopCanProceedStandard().Execute();

        var canProceed = Ioc.Resolve<bool>("Engine.Loop.CanProceed");
        Assert.True(canProceed);
    }

    [Fact]
    public void InitStrategy_ShouldSetupStartMetrics_AndLinkRuntime()
    {
        var containerMock = new Mock<ICommandContainer>();
        containerMock.SetupGet(c => c.Size).Returns(1);
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.Container", (object[] args) => (object)containerMock.Object).Execute();

        var setStartMetricMock = new Mock<ICommand>();
        
        // ИСПРАВЛЕНО: все внутренние зависимости регистрируем через "IoC.Register"
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CurrentTick.Get", (object[] args) => (object)500).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.StartTick.Set", (object[] args) => setStartMetricMock.Object).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed.Runtime", (object[] args) => (object)true).Execute();

        new RegisterLoopCanProceedInitialization().Execute();

        var stateResult = Ioc.Resolve<bool>("Engine.Loop.CanProceed.Init");

        Assert.True(stateResult);
        setStartMetricMock.Verify(m => m.Execute(), Times.Once);
        Assert.True(Ioc.Resolve<bool>("Engine.Loop.CanProceed"));
    }

    [Fact]
    public void RuntimeStrategy_ShouldReturnTrue_WhenTicksAreWithinLimit()
    {
        var containerMock = new Mock<ICommandContainer>();
        containerMock.SetupGet(c => c.Size).Returns(3);

        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.Container", (object[] args) => (object)containerMock.Object).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.FrameLimit", (object[] args) => (object)50).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.StartTick.Get", (object[] args) => (object)1000).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CurrentTick.Get", (object[] args) => (object)1020).Execute();

        new RegisterLoopCanProceedRuntime().Execute();

        Assert.True(Ioc.Resolve<bool>("Engine.Loop.CanProceed.Runtime"));
    }

    [Fact]
    public void RuntimeStrategy_ShouldFallbackToInit_WhenLimitExceeded()
    {
        var containerMock = new Mock<ICommandContainer>();
        containerMock.SetupGet(c => c.Size).Returns(3);

        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.Container", (object[] args) => (object)containerMock.Object).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.FrameLimit", (object[] args) => (object)10).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.StartTick.Get", (object[] args) => (object)1000).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CurrentTick.Get", (object[] args) => (object)1015).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed.Init", (object[] args) => (object)true).Execute();

        new RegisterLoopCanProceedRuntime().Execute();

        Assert.False(Ioc.Resolve<bool>("Engine.Loop.CanProceed.Runtime"));
        Assert.True(Ioc.Resolve<bool>("Engine.Loop.CanProceed"));
    }
}

