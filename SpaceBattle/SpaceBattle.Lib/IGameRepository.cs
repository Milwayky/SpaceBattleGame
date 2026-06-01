namespace SpaceBattle.Lib;

public interface IGameRepository
{
    void Save(IDictionary<string, object> entity);
    void Remove(string entityId);
    IDictionary<string, object> Retrieve(string entityId);
}

