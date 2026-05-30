namespace SpaceBattle.Lib;
using App;

public class RegisterDependencyCommandInjectableCommand : SpaceBattle.Lib.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.CommandInjectable", (object[] args) => 
            {
                return new CommandInjectableCommand();
            }
        ).Execute();
    }
}

