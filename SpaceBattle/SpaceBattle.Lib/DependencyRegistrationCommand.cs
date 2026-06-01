using App;

namespace SpaceBattle.Lib;

public class DependencyRegistrationCommand : ICommand
{
    private readonly IGameRepository _repo;

    public DependencyRegistrationCommand(IGameRepository repo) => _repo = repo;

    public void Execute()
    {
        // Регистрация команд через ваш IoC
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Entity.Add", 
            (object[] args) => new CreateGameEntityCommand(_repo, (IDictionary<string, object>)args[0])).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Entity.Remove", 
            (object[] args) => new DeleteGameEntityCommand(_repo, (string)args[0])).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Entity.Get", 
            (object[] args) => _repo.Retrieve((string)args[0])).Execute();
    }
}

