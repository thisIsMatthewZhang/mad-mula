using Godot;

public partial class Stalker(Player player) : CharacterBody3D
{

    [Signal]
    public delegate void HitPlayerEventHandler();
    private Player _player = player;

    public override void _PhysicsProcess(double delta)
    {
        
    }

    private void StalkPlayer() {}

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
