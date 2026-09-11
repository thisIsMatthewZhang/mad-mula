using Godot;

public partial class Stalker : CharacterBody3D
{

    [Signal]
    public delegate void HitPlayerEventHandler();
    private Player _player;

    public override void _Ready()
    {
        Position = new Vector3(_player.Position.X, _player.Position.Y, _player.Position.Z + 2.0f);
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

    private void StalkPlayer() {}

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    private void CheckCollisionWithPlayer()
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
