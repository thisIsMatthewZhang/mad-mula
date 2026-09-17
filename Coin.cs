using Godot;

public partial class Coin : AnimatableBody3D
{
    [Signal]
    public delegate void GrabbedEventHandler();
    private ShapeCast3D _shapeCast3DChild;

    public override void _Ready()
    {
        _shapeCast3DChild = GetNode<ShapeCast3D>("ShapeCast3D");
    }
    public override void _PhysicsProcess(double delta)
    {
        Spin();
        if (_shapeCast3DChild.IsColliding())
        {
            Position = new Vector3(Position.X, Position.Y + 0.5f, Position.Z);
        }
    }

    public void Remove()
    {
        // TODO: display sparkle
        EmitSignal(SignalName.Grabbed);
        QueueFree();
    }

    public void Spin() 
    {
        if (Rotation.Y >= Mathf.Tau / 2)
        {
            RotateY(0.0f);
        }
        RotateY(Mathf.Tau / 180);
    }
}
