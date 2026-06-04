using App;
using App.Scopes;
using SpaceBattle.Tests; 
using SpaceBattle.Lib;// Убедитесь, что здесь namespace ваших классов Register...

namespace SpaceBattle.Tests;

public class IocFixture
{
    public IocFixture()
    {
        // ВНИМАНИЕ: Здесь должен быть ТОЧНО такой же код, 
        // какой был в работающем тесте RegisterIoCDependencyMoveTests.
        // Если там был InitCommand, оставляем его, но дополняем тем,
        // что было в Setup того теста.
        
        new InitCommand().Execute();
        
        // ВОТ ТУТ СЕКРЕТ: добавьте сюда именно тот код, 
        // который регистрирует недостающие стратегии.
        // Обычно это выглядит как вызов команды регистрации:
        new RegisterIoCDependencyMoveCommandAuto().Execute();
        new RegisterIoCDependencyAutoAdapter().Execute();
    }
}


