using App;

namespace SpaceBattle.Lib;

public class StartCommand : App.ICommand
{
    private readonly IDictionary<string, object> _order;

    public StartCommand(IDictionary<string, object> order)
    {
        _order = order;
    }

    public void Execute()
    {
        var cmdName = (string)_order["command"];
        var cmdArgs = (object[])_order["args"];

        var cmd = Ioc.Resolve<ICommand>($"Commands.{cmdName}", cmdArgs);
        var injectable = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var receiver = Ioc.Resolve<ICommandReceiver>("Game.CommandsReceiver");

        var send = new SendCommand((ICommand)injectable, receiver);

        var repeatable = Ioc.Resolve<ICommand>($"Macro.{cmdName}", cmd, send);

        injectable.Inject(repeatable);

        _order[$"repeatable{cmdName}"] = injectable;

        var initialSend = new SendCommand(repeatable, receiver);

        initialSend.Execute();
    }
}

