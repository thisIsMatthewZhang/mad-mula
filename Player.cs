using System.Runtime.CompilerServices;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public int BaseSpeed { get; set; } = 5;

    private Vector3 _targetVelocity = Vector3.Zero;

    [Signal]
    public delegate void CoinGrabbedEventHandler();

    [Export]
    public int JumpImpulse { get; set; } = 20;

    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    public bool DEBUG_PLAYER_MOVEMENT_INFO { get; set; } = false;

    private bool _doubleJumpAllowed { get; set; } = false;

    private Vector3 _startingPosition;
    
    private Node3D _stairWalker;

    public override void _Ready()
    {
        _startingPosition = Position;
        _stairWalker = GetNode<Node3D>("StairWalker");
        GetNode<Main>("/root/Main").ChildEnteredTree += OnStalkerSpawned;
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;
        // var inputDirection = Input.GetVector("move_left", "move_right", "move_left", "ui_down");
        int actualSpeed;
        
        if (IsOnFloor()) // reset Y velocity to prevent 'push-down'
        {
            _targetVelocity.Y = 0;
        } 

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
        if (Input.IsActionPressed("run"))
        {
            actualSpeed = BaseSpeed * 2;
        }
        else
        {
            actualSpeed = BaseSpeed;
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

        _targetVelocity.X = actualSpeed * direction.X;
        _targetVelocity.Z = actualSpeed * direction.Z;

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

        // if (Math.Abs(inputDirection.Length()) > 0.1f)
        // {
        //     _stairWalker.RotateY((float)Math.Atan2(-direction.X, -direction.Y));
        //     // if (IsOnWall() && IsOnFloor() && _frontWallRaycast3D.IsColliding())
        //     // {
        //         // if (_stairRaycast3D.IsColliding() && !_frontRaycast3D.IsColliding()) {
        //         // var stepHeight = stairRayCast3D.GetCollisionPoint();
        //         // GlobalPosition = new Vector3(GlobalPosition.X, GlobalPosition.Y + stepHeight 0.05, GlobalPosition.Z);
        //         // }
        //     // }
        // }
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

    public void OnStalkerSpawned(Node node)
    {
        if (node is Stalker stalker)
        {
            stalker.HitPlayer += OnStalkerHitPlayer;
        }

        void OnStalkerHitPlayer()
        {
            var mat = GetNode<MeshInstance3D>("Pivot/MeshInstance3D").GetActiveMaterial(0) as StandardMaterial3D;
            var matDup = mat.Duplicate() as StandardMaterial3D;
            matDup.Roughness = 0.0f;
            matDup.Metallic = 1.0f;
            GetNode<MeshInstance3D>("Pivot/MeshInstance3D").SetSurfaceOverrideMaterial(0, matDup);
        }
    }
}
