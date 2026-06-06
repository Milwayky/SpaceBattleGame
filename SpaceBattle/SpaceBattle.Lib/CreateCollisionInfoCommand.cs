using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle.Lib;

public class CreateCollisionInfoCommand : ICommand
{
    private readonly HashSet<(int X, int Y)> _firstPoints;
    private readonly HashSet<(int X, int Y)> _secondPoints;
    private readonly IEnumerable<int> _relativeXs, _relativeYs, _relativeVxs, _relativeVys;
    public IEnumerable<(int, int, int, int)>? RelativeStates { get; private set; }

    public CreateCollisionInfoCommand(IEnumerable<Vector> fPoints, IEnumerable<Vector> sPoints, IEnumerable<int> rXs, IEnumerable<int> rYs, IEnumerable<int> rVxs, IEnumerable<int> rVys)
    {
        _firstPoints = ToPointsSet(fPoints); _secondPoints = ToPointsSet(sPoints);
        _relativeXs = rXs; _relativeYs = rYs; _relativeVxs = rVxs; _relativeVys = rVys;
    }

    public void Execute()
    {
        var states = new List<(int, int, int, int)>();
        foreach (var x in _relativeXs)
            foreach (var y in _relativeYs)
                foreach (var vx in _relativeVxs)
                    foreach (var vy in _relativeVys)
                        if (HasCollisionOnPath(x, y, vx, vy)) states.Add((x, y, vx, vy));
        RelativeStates = states;
    }

    private bool HasCollisionOnPath(int x, int y, int vx, int vy)
    {
        int steps = Math.Max(Math.Abs(vx), Math.Abs(vy));
        if (steps == 0) return _secondPoints.Any(p => _firstPoints.Contains((p.X + x, p.Y + y)));

        for (int i = 0; i <= steps; i++)
        {
            int cx = x + (int)Math.Round((double)vx * i / steps);
            int cy = y + (int)Math.Round((double)vy * i / steps);
            if (_secondPoints.Any(p => _firstPoints.Contains((p.X + cx, p.Y + cy)))) return true;
        }
        return false;
    }

    private static HashSet<(int X, int Y)> ToPointsSet(IEnumerable<Vector> points)
    {
        var field = typeof(Vector).GetField("_coordinates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return points.Select(p => {
            var coords = field?.GetValue(p) as int[];
            
            if (coords == null || coords.Length < 2)
            {
                throw new ArgumentException("Object points must be two-dimensional or field access failed.");
            }
            
            return (coords[0], coords[1]);
        }).ToHashSet();
    }
}

