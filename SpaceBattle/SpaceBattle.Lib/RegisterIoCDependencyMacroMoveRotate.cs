namespace SpaceBattle.Lib;

using App;
using System.Collections.Generic;

public class RegisterIoCDependencyMacroMoveRotate : ICommand
{
    public void Execute()
    {
        RegisterMacro("Macro.Move", "Specs.Move");
        RegisterMacro("Macro.Rotate", "Specs.Rotate");
    }

    private void RegisterMacro(string dependencyName, string specName)
    {
        dynamic register = Ioc.Resolve<object>(
            "IoC.Register",
            dependencyName,
            (object[] args) => new CreateMacroCommandStrategy(specName).Resolve(args)
        );
        register.Execute();
    }
}
