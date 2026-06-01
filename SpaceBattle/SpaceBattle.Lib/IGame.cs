using System.Collections.Generic;

namespace SpaceBattle.Lib;

public interface IGame
{
    void Update();
    IDictionary<string, object> State { get; }
}

