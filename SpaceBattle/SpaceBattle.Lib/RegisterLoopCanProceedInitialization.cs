using App;

namespace SpaceBattle.Lib;

public class RegisterLoopCanProceedInitialization : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed.Init", (object[] args) =>
        {
            var container = Ioc.Resolve<ICommandContainer>("Engine.Loop.Container");
            var initialTick = Ioc.Resolve<int>("Engine.Loop.CurrentTick.Get");

            Ioc.Resolve<ICommand>("Engine.Loop.StartTick.Set", initialTick).Execute();

            Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Engine.Loop.CanProceed",
                (object[] runtimeArgs) => (object)Ioc.Resolve<bool>("Engine.Loop.CanProceed.Runtime")
            ).Execute();

            return (object)(container.Size > 0);
        }).Execute();
    }
}

