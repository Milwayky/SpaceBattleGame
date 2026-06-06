namespace SpaceBattle.Lib;

public interface ICollisionObject
{
    string Form { get; }
    Vector Position { get; }
    Vector Velocity { get; }
}

public class CollisionObject : ICollisionObject
{
    private readonly IDictionary<string, object> _gameObject;

    public CollisionObject(IDictionary<string, object> gameObject)
    {
        _gameObject = gameObject;
    }

    public string Form => (string)GetValue("Form", "Type");
    public Vector Position => (Vector)GetValue("Position", "Location");
    public Vector Velocity => (Vector)GetValue("Velocity");

    private object GetValue(params string[] keys)
    {
        foreach (var key in keys)
        {
            if (_gameObject.ContainsKey(key)) return _gameObject[key];
        }
        return _gameObject[keys[0]];
    }
}

