using App;

namespace SpaceBattle.Lib;

public class RegisterAuthDependencies : ICommand
{
    private readonly IAuthRepository _repository;

    public RegisterAuthDependencies(IAuthRepository repository)
    {
        _repository = repository;
    }

    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Authorization.Check",
            (object[] args) => new AuthActionCommand(_repository, AuthActionType.Check, (string)args[0], (string)args[1], (string)args[2])
        ).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Authorization.Add",
            (object[] args) => new AuthActionCommand(_repository, AuthActionType.Add, (string)args[0], (string)args[1], (string)args[2])
        ).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Authorization.Remove",
            (object[] args) => new AuthActionCommand(_repository, AuthActionType.Remove, (string)args[0], (string)args[1], (string)args[2])
        ).Execute();
    }
}

