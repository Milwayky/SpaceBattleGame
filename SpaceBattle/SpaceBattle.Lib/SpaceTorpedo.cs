namespace SpaceBattle.Lib;

public class SpaceTorpedo : IShootable, IMoving, IWeaponized
{
    private const string PosKey = "Position";
    private const string VelKey = "Velocity";
    private const string DirKey = "Direction";

    private readonly IDictionary<string, object> _data;

    public SpaceTorpedo(IDictionary<string, object> data) => _data = data;

    public Vector Position
    {
        get => (Vector)_data[PosKey];
        set => _data[PosKey] = value;
    }

    public Vector Velocity
    {
        get => (Vector)_data[VelKey];
        set => _data[VelKey] = value;
    }

    public Angle Direction
    {
        get => (Angle)_data[DirKey];
        set => _data[DirKey] = value;
    }

    public Vector GetPosition() => Position;
    public Vector GetVelocity() => Velocity;
    public Angle GetDirection() => Direction;

    public void Initialize(Vector position, Vector velocity, Angle direction)
    {
        Position = position;
        _data[VelKey] = velocity;
        Direction = direction;
    }
}

