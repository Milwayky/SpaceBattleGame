namespace SpaceBattle.Lib;

using App;

public class RegisterIoCDependencyActionsStop : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Actions.Stop",
            (object[] args) =>
            {
                var order = (IDictionary<string, object>)args[0];
                return new StopCommand(order);
            }
        ).Execute();
    }
}

