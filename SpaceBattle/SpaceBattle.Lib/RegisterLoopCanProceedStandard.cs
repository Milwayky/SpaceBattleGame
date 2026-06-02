using App;

namespace SpaceBattle.Lib;

public class RegisterLoopCanProceedStandard : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Engine.Loop.CanProceed", (object[] args) =>
        {
            var container = Ioc.Resolve<ICommandContainer>("Engine.Loop.Container");
            return (object)(container.Size > 0);
        }).Execute();
    }
}

