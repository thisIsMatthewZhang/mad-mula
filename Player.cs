using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public int Speed { get; set; } = 5;

    private Vector3 _targetVelocity = Vector3.Zero;
    [Signal]
    public delegate void CoinGrabbedEventHandler();

    [Export]
    public int JumpImpulse { get; set; } = 20;

    [Export]
    public int FallAcceleration { get; set; } = 75;
    private bool _doubleJumpAllowed { get; set; } = false;

    private Vector3 _startingPosition;

    public override void _Ready()
    {
        _startingPosition = Position;
    }

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
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
            _doubleJumpAllowed = true;
            
        }
        if (!IsOnFloor() && Input.IsActionJustPressed("jump") && _doubleJumpAllowed)
        {
            _targetVelocity.Y = JumpImpulse * 0.75f;
            _doubleJumpAllowed = false;
        }

        _targetVelocity.X = Speed * direction.X;
        _targetVelocity.Z = Speed * direction.Z;

        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float) delta;
        }

        if (Position.Y < -50.0f)
        {
            _targetVelocity.Y = 0;
            Position = _startingPosition;
        }

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
                EmitSignal(SignalName.CoinGrabbed);
            }
        }
    }
}
