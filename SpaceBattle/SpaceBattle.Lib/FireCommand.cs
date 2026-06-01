namespace SpaceBattle.Lib;

public class FireCommand : ICommand
{
    private readonly IShootable _shooter;
    private readonly IWeaponized _bullet;

    public FireCommand(IShootable shooter, IWeaponized bullet)
    {
        _shooter = shooter;
        _bullet = bullet;
    }

    public void Execute()
    {
        _bullet.Initialize(
            _shooter.GetPosition(), 
            _shooter.GetVelocity(), 
            _shooter.GetDirection()
        );
    }
}

