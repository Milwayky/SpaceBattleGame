using App;

namespace SpaceBattle.Lib;

public class RegisterIoCDependencyAutoAdapter : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Adapters.IMoving",
            (object[] args) =>
            {
                var dict = (IDictionary<string, object>)args[0];
                return AdapterBuilder.Build<IMoving>(dict);
            }
        ).Execute();

        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Adapters.IRotating",
            (object[] args) =>
            {
                var dict = (IDictionary<string, object>)args[0];
                return AdapterBuilder.Build<IRotating>(dict);
            }
        ).Execute();
    }
}