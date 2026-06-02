using App;

namespace SpaceBattle.Lib;

public class RegisterLoopCanProceedRuntime : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed.Runtime", (object[] args) =>
        {
            var container = Ioc.Resolve<ICommandContainer>("Engine.Loop.Container");
            var frameLimit = Ioc.Resolve<int>("Engine.Loop.FrameLimit");
            var startTick = Ioc.Resolve<int>("Engine.Loop.StartTick.Get");
            var currentTick = Ioc.Resolve<int>("Engine.Loop.CurrentTick.Get");

            bool hasCapacity = (startTick + frameLimit) > currentTick && container.Size > 0;

            if (!hasCapacity)
            {
                Ioc.Resolve<App.ICommand>(
                    "IoC.Register",
                    "Engine.Loop.CanProceed",
                    (object[] fallbackArgs) => (object)Ioc.Resolve<bool>("Engine.Loop.CanProceed.Init")
                ).Execute();
            }

            return (object)hasCapacity;
        }).Execute();
    }
}

