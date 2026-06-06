using App;

namespace SpaceBattle.Lib;

public class StopCommand : App.ICommand
{
    private readonly IDictionary<string, object> _order;

    public StopCommand(IDictionary<string, object> order)
    {
        _order = order;
    }

    public void Execute()
    {
        var cmdName = (string)_order["command"];
        string repeatableKey = $"repeatable{cmdName}";

        if (!_order.ContainsKey(repeatableKey))
        {
            throw new InvalidOperationException($"Command {cmdName} hasn't been started");
        }

        var injectable = (ICommandInjectable)_order[repeatableKey];

        injectable.Inject(new EmptyCommand());
        _order.Remove(repeatableKey);
    }
}

