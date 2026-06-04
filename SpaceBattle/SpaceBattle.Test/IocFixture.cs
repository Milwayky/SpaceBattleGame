using App;
using App.Scopes;
using SpaceBattle.Tests; 
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class IocFixture
{
    public IocFixture()
    {
        new InitCommand().Execute();
        
        new RegisterIoCDependencyMoveCommandAuto().Execute();
        new RegisterIoCDependencyAutoAdapter().Execute();
    }
}

