using App.Scopes;

namespace SpaceBattle.Tests;

public class IocFixture
{
    public IocFixture()
    {
        // Инициализируем IoC один раз для всех тестов в этом классе
        new InitCommand().Execute();
    }
}