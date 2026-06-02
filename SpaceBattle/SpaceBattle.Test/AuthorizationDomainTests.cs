
using App;
using App.Scopes;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Tests;

[Collection("SequentialTests")]
public class AuthorizationDomainTests
{
    #region Универсальные Тесты Команд (AuthActionCommand)

    [Fact]
    public void Execute_AddPermission_ShouldInvokeRepository()
    {
        var repoMock = new Mock<IAuthRepository>();
        var command = new AuthActionCommand(repoMock.Object, AuthActionType.Add, "1", "10", "Move");

        command.Execute();

        repoMock.Verify(r => r.AddPermission("1", "10", "Move"), Times.Once);
    }

    [Fact]
    public void Execute_CheckPermission_ShouldPass_WhenAllowed()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckPermission("1", "10", "Move")).Returns(true);
        var command = new AuthActionCommand(repoMock.Object, AuthActionType.Check, "1", "10", "Move");

        var exception = Record.Exception(() => command.Execute());
        
        Assert.Null(exception);
    }

    [Fact]
    public void Execute_CheckPermission_ShouldThrow_WhenDenied()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckPermission("1", "10", "Shoot")).Returns(false);
        var command = new AuthActionCommand(repoMock.Object, AuthActionType.Check, "1", "10", "Shoot");

        Assert.Throws<UnauthorizedAccessException>(() => command.Execute());
    }

    [Fact]
    public void Execute_RemovePermission_ShouldThrow_WhenNotFound()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.RemovePermission("1", "10", "Move")).Throws<KeyNotFoundException>();
        var command = new AuthActionCommand(repoMock.Object, AuthActionType.Remove, "1", "10", "Move");

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
    }

    #endregion

    #region Тесты Дерева Авторизации (AuthTreeRepository)

    [Fact]
    public void TreeRepository_ShouldManagePermissionsCorrectly()
    {
        var tree = new Dictionary<string, IDictionary<string, HashSet<string>>>();
        var repo = new AuthTreeRepository(tree);

        repo.AddPermission("1", "10", "Move");
        Assert.True(repo.CheckPermission("1", "10", "Move"));
        Assert.False(repo.CheckPermission("1", "10", "Shoot"));

        repo.RemovePermission("1", "10", "Move");
        Assert.False(repo.CheckPermission("1", "10", "Move"));
    }

    [Fact]
    public void TreeRepository_RemoveNonExistent_ShouldThrowKeyNotFoundException()
    {
        var tree = new Dictionary<string, IDictionary<string, HashSet<string>>>();
        var repo = new AuthTreeRepository(tree);

        Assert.Throws<KeyNotFoundException>(() => repo.RemovePermission("UserX", "ObjY", "Fly"));
    }

    #endregion

    #region Тесты Инфраструктуры IoC

    [Fact]
    public void IoCRegistration_ShouldResolveCorrectCommands()
    {
        // Инициализируем окружение IoC
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();

        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckPermission("1", "10", "Move")).Returns(false);

        var registration = new RegisterAuthDependencies(repoMock.Object);
        registration.Execute();

        var checkCmd = Ioc.Resolve<App.ICommand>("Authorization.Check", "1", "10", "Move");
        var addCmd = Ioc.Resolve<App.ICommand>("Authorization.Add", "1", "10", "Move");
        var removeCmd = Ioc.Resolve<App.ICommand>("Authorization.Remove", "1", "10", "Move");

        Assert.IsType<AuthActionCommand>(checkCmd);
        Assert.IsType<AuthActionCommand>(addCmd);
        Assert.IsType<AuthActionCommand>(removeCmd);
        Assert.Throws<UnauthorizedAccessException>(() => checkCmd.Execute());
    }

    #endregion
}

