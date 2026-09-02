using Godot;
using System;

public partial class Coin : AnimatableBody3D
{
    public override void _PhysicsProcess(double delta)
    {
        Spin();
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
