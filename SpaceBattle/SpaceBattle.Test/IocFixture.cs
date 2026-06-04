using App.Scopes;
using SpaceBattle.Lib; // <--- ДОБАВЬТЕ ЭТОТ USING (замените на реальный namespace)

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

