using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle.Lib;

public class CollisionTree
{
    private readonly Dictionary<int, Dictionary<int, Dictionary<int, HashSet<int>>>> _tree = new();

    public CollisionTree(IEnumerable<(int, int, int, int)> relativeStates)
    {
        foreach (var state in relativeStates)
        {
            Add(state);
        }
    }

    public void Add((int, int, int, int) state)
    {
        if (!_tree.TryGetValue(state.Item1, out var level1))
        {
            level1 = new Dictionary<int, Dictionary<int, HashSet<int>>>();
            _tree[state.Item1] = level1;
        }

        if (!level1.TryGetValue(state.Item2, out var level2))
        {
            level2 = new Dictionary<int, HashSet<int>>();
            level1[state.Item2] = level2;
        }

        if (!level2.TryGetValue(state.Item3, out var level3))
        {
            level3 = new HashSet<int>();
            level2[state.Item3] = level3;
        }

        level3.Add(state.Item4);
    }

    public bool Contains((int, int, int, int) state)
    {
        return _tree.TryGetValue(state.Item1, out var level1) &&
               level1.TryGetValue(state.Item2, out var level2) &&
               level2.TryGetValue(state.Item3, out var level3) &&
               level3.Contains(state.Item4);
    }
}

