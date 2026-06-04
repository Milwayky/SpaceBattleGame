namespace SpaceBattle.Lib;

using App;

public class RegisterIoCDependencyRotateCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Rotate", (object[] args) =>
        {
            var obj = Ioc.Resolve<IRotating>("Adapters.IRotating", args[0]);

            return new RotateCommand(obj);
        }).Execute();
    }
}

