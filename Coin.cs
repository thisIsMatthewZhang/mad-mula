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
