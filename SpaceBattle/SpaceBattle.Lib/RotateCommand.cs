using SpaceBattle.Lib;

namespace SpaceBattle.Lib;

public class RotateCommand : ICommand
{
    private readonly IRotating _rotating;

    public RotateCommand(IRotating rotating)
    {
        _rotating = rotating;
    }

    public void Execute()
    {
        _rotating.Angle = _rotating.Angle + _rotating.AngularVelocity;
    }
}

