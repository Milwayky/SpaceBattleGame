namespace SpaceBattle.Lib;

using App;

public class RegisterIoCDependencyActionsStart : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Actions.Start",
            (object[] args) =>
            {
                var order = (IDictionary<string, object>)args[0];
                return new StartCommand(order);
            }
        ).Execute();
    }
}

