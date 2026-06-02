using App;

namespace SpaceBattle.Lib;

public interface ICommandContainer
{
    int Size { get; }
    void Push(ICommand command);
    ICommand Pull();
}

