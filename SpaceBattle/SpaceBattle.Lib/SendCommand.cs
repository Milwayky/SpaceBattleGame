namespace SpaceBattle.Lib;

public class SendCommand : ICommand
{
    private readonly ICommand _cmd;
    private readonly ICommandReceiver _receiver;

    public SendCommand(ICommand cmd, ICommandReceiver receiver)
    {
        _cmd = cmd;
        _receiver = receiver;
    }

    public void Execute()
    {
        _receiver.Receive(_cmd);
    }
}

