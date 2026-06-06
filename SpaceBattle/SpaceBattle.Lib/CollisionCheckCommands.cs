using System;
using System.Collections.Generic;

namespace SpaceBattle.Lib;

public class CheckCollisionCommand : ICommand
{
    private readonly ICollisionObject _firstObject;
    private readonly ICollisionObject _secondObject;
    private readonly IDictionary<(string, string), CollisionTree> _storage;

    public bool HasCollision { get; private set; }

    public CheckCollisionCommand(ICollisionObject firstObject, ICollisionObject secondObject, IDictionary<(string, string), CollisionTree> storage)
    {
        _firstObject = firstObject;
        _secondObject = secondObject;
        _storage = storage;
    }

    public void Execute()
    {
        var directState = GetRelativeState(_firstObject, _secondObject);

        if (_storage.TryGetValue((_firstObject.Form, _secondObject.Form), out var directTree))
        {
            HasCollision = directTree.Contains(directState);
            return;
        }

        if (_storage.TryGetValue((_secondObject.Form, _firstObject.Form), out var reverseTree))
        {
            HasCollision = reverseTree.Contains(GetRelativeState(_secondObject, _firstObject));
            return;
        }

        HasCollision = false;
    }

    private static (int, int, int, int) GetRelativeState(ICollisionObject firstObject, ICollisionObject secondObject)
    {
        var firstPos = GetVectorCoordinates(firstObject.Position);
        var secondPos = GetVectorCoordinates(secondObject.Position);
        var firstVel = GetVectorCoordinates(firstObject.Velocity);
        var secondVel = GetVectorCoordinates(secondObject.Velocity);

        return (secondPos[0] - firstPos[0], secondPos[1] - firstPos[1], secondVel[0] - firstVel[0], secondVel[1] - firstVel[1]);
    }

    private static int[] GetVectorCoordinates(Vector vector)
    {
        var fieldInfo = typeof(Vector).GetField("_coordinates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo?.GetValue(vector) is int[] coordinates && coordinates.Length >= 2)
        {
            return coordinates;
        }
        throw new ArgumentException("Collision detection requires 2D vectors.");
    }
}

public class CheckCollisionsCommand : ICommand
{
    private readonly ICollisionObject _target;
    private readonly IEnumerable<ICollisionObject> _objects;
    private readonly IDictionary<(string, string), CollisionTree> _storage;

    public IReadOnlyCollection<ICollisionObject> CollidedObjects { get; private set; } = Array.Empty<ICollisionObject>();

    public CheckCollisionsCommand(ICollisionObject target, IEnumerable<ICollisionObject> objects, IDictionary<(string, string), CollisionTree> storage)
    {
        _target = target;
        _objects = objects;
        _storage = storage;
    }

    public void Execute()
    {
        var collided = new List<ICollisionObject>();
        foreach (var obj in _objects)
        {
            if (ReferenceEquals(_target, obj)) continue;

            var check = new CheckCollisionCommand(_target, obj, _storage);
            check.Execute();

            if (check.HasCollision) collided.Add(obj);
        }
        CollidedObjects = collided;
    }
}

