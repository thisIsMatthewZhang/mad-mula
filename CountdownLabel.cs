using Godot;
using System;

public partial class CountdownLabel : Label
{
    public void UpdateTimeRemaining(double time)
    {
        Text = $"Time left: {time}";
    }
}
