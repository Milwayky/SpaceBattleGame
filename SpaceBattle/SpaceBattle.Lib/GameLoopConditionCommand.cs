using App;

namespace SpaceBattle.Lib;

public class GameLoopConditionCommand : ICommand
{
    public void Execute()
    {
        var queue = Ioc.Resolve<IGameQueue>("Game.Queue");
        var startTime = Ioc.Resolve<int>("Game.Loop.StartTime.Get");

        Ioc.Resolve<ICommand>("Game.Loop.StartTime.Set", startTime).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Loop.ShouldContinue", (object[] args) => 
            Ioc.Resolve<bool>("Game.Loop.CheckStatus")).Execute();
    }
}

