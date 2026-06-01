using App;

namespace SpaceBattle.Lib;

public class RegisterSchedulerDependencies : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Loop.ShouldContinue", (object[] args) => 
            (object)(Ioc.Resolve<IGameQueue>("Game.Queue").Count > 0)).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Loop.DequeueCommand", (object[] args) => 
            Ioc.Resolve<IGameQueue>("Game.Queue").Dequeue()).Execute();
    }
}

