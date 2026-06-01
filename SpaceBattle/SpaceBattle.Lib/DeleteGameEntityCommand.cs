using App;

namespace SpaceBattle.Lib;

public class DeleteGameEntityCommand : ICommand
{
    private readonly IGameRepository _repo;
    private readonly string _id;

    public DeleteGameEntityCommand(IGameRepository repo, string id)
    {
        _repo = repo;
        _id = id;
    }

    public void Execute() => _repo.Remove(_id);
}

