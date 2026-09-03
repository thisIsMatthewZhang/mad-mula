using Godot;
using System;

public partial class Coin : AnimatableBody3D
{
    [Signal]
    public delegate void GrabbedEventHandler();
    public override void _PhysicsProcess(double delta)
    {
        Spin();
    }

    public Coin Remove()
    {
        // TODO: display sparkle effects
        EmitSignal(SignalName.Grabbed);
        QueueFree();
        return this;
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
