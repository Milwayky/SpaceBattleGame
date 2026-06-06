using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle.Lib;

public class CheckCollisionsCommand : ICommand
{
    private readonly ICollisionObject _target;
    private readonly IEnumerable<ICollisionObject> _others;
    private readonly IDictionary<(string, string), CollisionTree> _storage;

    public bool HasCollision { get; private set; }
    public IReadOnlyCollection<ICollisionObject> CollidedObjects { get; private set; } = Array.Empty<ICollisionObject>();

    public CheckCollisionsCommand(ICollisionObject target, IEnumerable<ICollisionObject> others, IDictionary<(string, string), CollisionTree> storage)
    {
        _target = target;
        _others = others;
        _storage = storage;
    }

    public void Execute()
    {
        var collided = new List<ICollisionObject>();
        foreach (var obj in _others)
        {
            if (ReferenceEquals(_target, obj)) continue;

            if (_storage.TryGetValue((_target.Form, obj.Form), out var tree))
            {
                if (tree.Contains(GetRelativeState(_target, obj)))
                {
                    collided.Add(obj);
                }
            }
        }
        CollidedObjects = collided;
        HasCollision = collided.Any();
    }

    private static (int, int, int, int) GetRelativeState(ICollisionObject first, ICollisionObject second)
    {
        var fPos = GetCoords(first.Position);
        var sPos = GetCoords(second.Position);
        var fVel = GetCoords(first.Velocity);
        var sVel = GetCoords(second.Velocity);

        return (sPos[0] - fPos[0], sPos[1] - fPos[1], sVel[0] - fVel[0], sVel[1] - fVel[1]);
    }

    private static int[] GetCoords(Vector vector)
    {
        var field = typeof(Vector).GetField("_coordinates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var value = field?.GetValue(vector);
        
        if (value is int[] coords)
        {
            if (coords.Length < 2) 
                throw new ArgumentException("Vector dimension must be at least 2");
            return coords;
        }
        
        throw new ArgumentException("Vector does not contain valid coordinate array");
    }
}

