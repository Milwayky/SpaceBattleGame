using System.Collections.Generic;

namespace SpaceBattle.Lib;

public class AuthTreeRepository : IAuthRepository
{
    private readonly IDictionary<string, IDictionary<string, HashSet<string>>> _tree;

    public AuthTreeRepository(IDictionary<string, IDictionary<string, HashSet<string>>> tree)
    {
        _tree = tree;
    }

    public bool CheckPermission(string subjectId, string objectId, string action) =>
        _tree.TryGetValue(subjectId, out var objects) &&
        objects.TryGetValue(objectId, out var actions) &&
        actions.Contains(action);

    public void AddPermission(string subjectId, string objectId, string action)
    {
        if (!_tree.TryGetValue(subjectId, out var objects))
        {
            objects = new Dictionary<string, HashSet<string>>();
            _tree[subjectId] = objects;
        }

        if (!objects.TryGetValue(objectId, out var actions))
        {
            actions = new HashSet<string>();
            objects[objectId] = actions;
        }

        actions.Add(action);
    }

    public void RemovePermission(string subjectId, string objectId, string action)
    {
        if (!CheckPermission(subjectId, objectId, action))
        {
            throw new KeyNotFoundException($"Permission for subject '{subjectId}' on object '{objectId}' with action '{action}' was not found.");
        }

        _tree[subjectId][objectId].Remove(action);
    }
}

