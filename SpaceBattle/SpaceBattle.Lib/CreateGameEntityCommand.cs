using App;

namespace SpaceBattle.Lib;

public class CreateGameEntityCommand : ICommand
{
    private readonly IGameRepository _repo;
    private readonly IDictionary<string, object> _data;

    public CreateGameEntityCommand(IGameRepository repo, IDictionary<string, object> data)
    {
        _repo = repo;
        _data = data;
    }

    public void Execute()
    {
        if (!_data.ContainsKey(InMemoryGameRepository.EntityIdKey))
        {
            _data[InMemoryGameRepository.EntityIdKey] = Guid.NewGuid().ToString();
        }
        _repo.Save(_data);
    }
}

