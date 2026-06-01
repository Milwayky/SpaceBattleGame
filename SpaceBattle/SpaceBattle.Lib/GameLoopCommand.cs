using App;

namespace SpaceBattle.Lib;

public class GameLoopCommand : ICommand
{
    private readonly object _scope;

    public GameLoopCommand(object scope) => _scope = scope;

    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Scope.Current.Set", _scope).Execute();

        while (Ioc.Resolve<bool>("Game.Loop.ShouldContinue"))
        {
            var cmd = Ioc.Resolve<ICommand>("Game.Loop.DequeueCommand");
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                Ioc.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
            }
        }
    }
}

