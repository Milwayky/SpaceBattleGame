using App;

namespace SpaceBattle.Lib;

public class UpdateGameCommand : ICommand
{
    private readonly IGame _game;

    public UpdateGameCommand(IGame game) => _game = game;

    public void Execute()
    {
        _game.Update();
    }
}
