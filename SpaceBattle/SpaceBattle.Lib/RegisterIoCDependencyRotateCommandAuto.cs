using App;

namespace SpaceBattle.Lib;

public class RegisterIoCDependencyRotateCommandAuto : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Commands.Rotate",
            (object[] args) =>
            {
                var obj = args[0];
                var rotatingObject = Ioc.Resolve<IRotating>("Adapters.IRotating", obj);
                return new RotateCommand(rotatingObject);
            }
        ).Execute();
    }
}

