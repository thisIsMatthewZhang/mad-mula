using Godot;
using System;

public partial class TimerLabel : Label
{
    public void UpdateTimeRemaining(double time)
    {
        Text = $"Time left: {time}";
    }
}
