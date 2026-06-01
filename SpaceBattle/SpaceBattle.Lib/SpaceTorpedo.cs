using App;

namespace SpaceBattle.Lib;

public class SpaceTorpedo : IShootable, IMoving
{
    private const string PosKey = "Position";
    private const string VelKey = "Velocity";
    private const string DirKey = "Direction";

    private readonly IDictionary<string, object> _data;

    public SpaceTorpedo(IDictionary<string, object> data) => _data = data;

    public Vector Position { get => (Vector)_data[PosKey]; set => UpdateOrAddData(PosKey, value); }
    public Vector Velocity { get => (Vector)_data[VelKey]; set => UpdateOrAddData(VelKey, value); }
    public Angle Direction { get => (Angle)_data[DirKey]; set => UpdateOrAddData(DirKey, value); }

    public Vector GetPosition() => Position;
    public Vector GetVelocity() => Velocity;
    public Angle GetDirection() => Direction;

    private void UpdateOrAddData(string key, object value)
    {
        if (_data.TryGetValue(key, out _)) _data[key] = value;
        else _data.Add(key, value);
    }
}

