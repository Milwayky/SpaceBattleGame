using App;

namespace SpaceBattle.Lib;

public interface IGameQueue
{
    int Count { get; }
    void Enqueue(ICommand command);
    ICommand Dequeue();
}

