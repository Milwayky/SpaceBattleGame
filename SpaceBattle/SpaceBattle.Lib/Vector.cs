namespace SpaceBattle.Lib;

public class Vector
{
    private readonly int[] _coordinates;

    public Vector(params int[] coordinates)
    {
        _coordinates = (int[])coordinates.Clone();
    }

    public int Dimension => _coordinates.Length;

    public static Vector operator +(Vector v1, Vector v2)
    {
        if (v1.Dimension != v2.Dimension)
        {
            throw new ArgumentException(); 
        }
        
        var sumCoordinates = v1._coordinates.Zip(v2._coordinates, (a, b) => a + b).ToArray();
        return new Vector(sumCoordinates);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Vector other) return false;
        if (this.Dimension != other.Dimension) return false;
        return _coordinates.SequenceEqual(other._coordinates);
    }

    public static bool operator ==(Vector? v1, Vector? v2) => Equals(v1, v2);
    public static bool operator !=(Vector? v1, Vector? v2) => !Equals(v1, v2);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var coord in _coordinates)
        {
            hash.Add(coord);
        }
        return hash.ToHashCode();
    }
}     

