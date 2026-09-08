using Godot;

public partial class CountdownLabel : Label
{
    public void DecrementCountdown(double time)
    {
        Text = $"{time--}";
    }
}
