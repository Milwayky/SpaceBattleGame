namespace SpaceBattle.Lib;

using System.Collections.Generic;
using System.Linq;

public class MacroCommand : ICommand
{
    private readonly IEnumerable<ICommand> _commands;

    public MacroCommand(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }

    public void Execute()
{
    ExecuteCommands(_commands.GetEnumerator());
}

    private void ExecuteCommands(IEnumerator<ICommand> enumerator)
    {
        if (enumerator.MoveNext())
        {
            enumerator.Current.Execute();
            ExecuteCommands(enumerator);
        }
    }
}

