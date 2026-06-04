namespace SpaceBattle.Lib;

public class Angle
{
    public static int d { get; set; } = 8;
    public int n { get; }

    public Angle(int n)
    {
        this.n = (n % d + d) % d;
    }

    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle(a.n + b.n);
    }

    public static implicit operator double(Angle a)
    {
        return 2 * Math.PI * a.n / d;
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle angle && n == angle.n;
    }

    public override int GetHashCode()
    {
        return n.GetHashCode();
    }

    public static bool operator ==(Angle? a, Angle? b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(Angle? a, Angle? b)
    {
        return !Equals(a, b);
    }
}

