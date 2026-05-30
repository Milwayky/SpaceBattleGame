namespace SpaceBattle.Lib;
using App;
using System.Collections.Generic;
using System.Linq;

public class CreateMacroCommandStrategy
{
    private readonly string _commandSpec;

    public CreateMacroCommandStrategy(string commandSpec)
    {
        _commandSpec = commandSpec;
    }

    public SpaceBattle.Lib.ICommand Resolve(params object[] args)
    {
        var target = args[0];
        var commandNames = Ioc.Resolve<IEnumerable<string>>(_commandSpec);
        var commands = commandNames
            .Select(name => Ioc.Resolve<SpaceBattle.Lib.ICommand>(name, target))
            .ToList();
        return Ioc.Resolve<SpaceBattle.Lib.ICommand>("Commands.Macro", commands);
    }
}

