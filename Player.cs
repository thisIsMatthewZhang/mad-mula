using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    public int Speed { get; set; } = 5;

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        // X and Z refer to the ground plane in 3D
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }
        _targetVelocity.X = Speed * direction.X;
        _targetVelocity.Z = Speed * direction.Z;

        Velocity = _targetVelocity;
        MoveAndSlide();
        CheckCollisionWithCoin();
    }
    public void CheckCollisionWithCoin()
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++) {
            KinematicCollision3D collision = GetSlideCollision(i);
            if (collision.GetCollider() is Coin coin) {
                coin.Remove();
            }
        }
    }
}
