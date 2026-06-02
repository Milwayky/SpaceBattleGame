using App;

namespace SpaceBattle.Lib;

public class GameLoopCommand : ICommand
{
    private readonly object _engineContext;

    public GameLoopCommand(object engineContext)
    {
        _engineContext = engineContext;
    }

    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _engineContext).Execute();

        while (Ioc.Resolve<bool>("Engine.Loop.CanProceed"))
        {
            var nextCmd = Ioc.Resolve<ICommand>("Engine.Loop.FetchNext");

            try
            {
                nextCmd.Execute();
            }
            catch (Exception ex)
            {
                Ioc.Resolve<ICommand>("Engine.Errors.Handle", nextCmd, ex).Execute();
            }
        }
    }
}

