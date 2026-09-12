using Godot;

public partial class Stalker : CharacterBody3D
{

    [Signal]
    public delegate void HitPlayerEventHandler();
    private Player _player;

    public override void _Ready() {}

    public override void _PhysicsProcess(double delta)
    {
        _stalkPlayer();
        MoveAndSlide(); // need to call this to enable proper slide collision detection
        _checkCollisionWithPlayer();
    }

    private void _stalkPlayer()
    {
        float speed = 0.015f;
        LookAt(_player.GlobalPosition);
        float xLerped = Mathf.Lerp(Position.X, _player.GlobalPosition.X, speed);
        float yLerped = Mathf.Lerp(Position.Y, _player.GlobalPosition.Y, speed);
        float zLerped = Mathf.Lerp(Position.Z, _player.GlobalPosition.Z, speed);
        Position = new Vector3(xLerped, yLerped, zLerped);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    private void _checkCollisionWithPlayer()
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);
            if (collision.GetCollider() is Player)
            {
                EmitSignal(SignalName.HitPlayer);
            }
        }
    }
}
