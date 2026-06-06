using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpaceBattle.Lib;

public class ReadCollisionInfoFileCommand : ICommand
{
    private readonly string _filePath;
    public IEnumerable<(int, int, int, int)>? Quads { get; private set; }

    public ReadCollisionInfoFileCommand(string filePath) => _filePath = filePath;

    public void Execute()
    {
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _filePath);
        Quads = File.ReadAllLines(fullPath)
            .Select(l => l.Split(new[] { ',', ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries))
            .Where(p => p.Length == 4)
            .Select(p => (int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3])))
            .ToList();
    }
}

public class WriteCollisionInfoFileCommand : ICommand
{
    private readonly string _filePath;
    private readonly IEnumerable<(int, int, int, int)> _relativeStates;

    public WriteCollisionInfoFileCommand(string filePath, IEnumerable<(int, int, int, int)> states)
    {
        _filePath = filePath; _relativeStates = states;
    }

    public void Execute() => File.WriteAllLines(_filePath, _relativeStates.Select(s => $"{s.Item1},{s.Item2},{s.Item3},{s.Item4}"));
}

