namespace SpaceBattle.Lib;

using App;
using System.Collections.Generic;

public class RegisterIoCDependencyMacroCommand : ICommand
{
    public void Execute()
    {
        dynamic registerCommand = Ioc.Resolve<object>("IoC.Register", "Commands.Macro", (object[] args) => 
            {
                var commands = (IEnumerable<SpaceBattle.Lib.ICommand>)args[0];
                return new MacroCommand(commands);
            }
        );
        
        registerCommand.Execute();
    }
}

