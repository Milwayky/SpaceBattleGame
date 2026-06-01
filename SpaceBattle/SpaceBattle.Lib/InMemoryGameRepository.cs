using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class InMemoryGameRepository : IGameRepository
{
    private readonly ConcurrentDictionary<string, IDictionary<string, object>> _items = new();
    public const string EntityIdKey = "id";

    public void Save(IDictionary<string, object> entity)
    {
        if (!entity.ContainsKey(EntityIdKey))
            throw new ArgumentException("Entity must have an ID.");
            
        _items[(string)entity[EntityIdKey]] = entity;
    }

    public void Remove(string entityId) => _items.TryRemove(entityId, out _);

    public IDictionary<string, object> Retrieve(string entityId) => _items[entityId];
}

