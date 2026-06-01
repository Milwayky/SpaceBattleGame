using App;
using System;
using System.Collections.Generic;

namespace SpaceBattle.Lib;

public class Game : IGame
{
    public IDictionary<string, object> State { get; }

    public Game(IDictionary<string, object> state)
    {
        State = state;
    }

    public void Update()
    {
        var currentTick = State.TryGetValue("Tick", out var tick) ? (int)tick : 0;
        State["Tick"] = currentTick + 1;

        State["LastUpdated"] = DateTime.UtcNow;

        if (State.TryGetValue("GameEvents", out var eventsObj) && eventsObj is Queue<ICommand> gameEvents)
        {
            var eventsToProcess = gameEvents.Count;
            for (var i = 0; i < eventsToProcess; i++)
            {
                var cmd = gameEvents.Dequeue();
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

        if (State.TryGetValue("MaxTicks", out var maxTicksObj) && maxTicksObj is int maxTicks)
        {
            if ((int)State["Tick"] >= maxTicks)
            {
                State["IsGameOver"] = true;
            }
        }
    }
}

