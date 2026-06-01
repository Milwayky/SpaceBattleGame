using App;
using App.Scopes;
using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class DependencyRegistrationCommandTests
{
    private readonly Mock<IGameRepository> _repoMock;

    public DependencyRegistrationCommandTests()
    {
        new InitCommand().Execute(); 
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        
        Ioc.Resolve<ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
                
        _repoMock = new Mock<IGameRepository>();
    }

    [Fact]
    public void Registration_ShouldRegisterAllCommands()
    {
        var cmd = new DependencyRegistrationCommand(_repoMock.Object);
        cmd.Execute();

        var testEntity = new Dictionary<string, object> { { "id", "1" } };
        var createdCommand = Ioc.Resolve<ICommand>("Game.Entity.Add", testEntity);
        
        Assert.NotNull(createdCommand);
    }
}

