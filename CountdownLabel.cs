using Godot;
using System;

public partial class CountdownLabel : Label
{
    public void DecrementCountdown(double time)
    {
        Text = $"{time--}";
    }
}
