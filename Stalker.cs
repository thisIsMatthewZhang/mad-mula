using Godot;

public partial class Stalker : CharacterBody3D
{

    [Signal]
    public delegate void HitPlayerEventHandler();
    private Player _player;

    public override void _Ready() {}

    public override void _PhysicsProcess(double delta)
    {
        StalkPlayer();
    }

    private void StalkPlayer()
    {
        
    }

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
