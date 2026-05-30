namespace SpaceBattle.Lib;

using App;

public class RegisterIoCDependencySendCommand : SpaceBattle.Lib.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Send", (object[] args) => 
        
            {
                var commandToSend = (SpaceBattle.Lib.ICommand)args[0];
                var receiver = (ICommandReceiver)args[1];

                return new SendCommand(commandToSend, receiver);
            }
        ).Execute();
    }
}

