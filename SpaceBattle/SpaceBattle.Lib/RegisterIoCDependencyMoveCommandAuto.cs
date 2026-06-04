using App;

namespace SpaceBattle.Lib;

public class RegisterIoCDependencyMoveCommandAuto : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Commands.Move",
            (object[] args) =>
            {
                var obj = args[0];
                var movingObject = Ioc.Resolve<IMoving>("Adapters.IMoving", obj);
                return new MoveCommand(movingObject);
            }
        ).Execute();
    }
}

