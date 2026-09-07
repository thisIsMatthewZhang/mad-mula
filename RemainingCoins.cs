using Godot;

public partial class RemainingCoins : Label
{
    [Signal]
    public delegate void AllGrabbedEventHandler();
    public int CoinCount { get; set; }

    public void OnCoinGrabbed()
    {
        Text = $"Coins left: {--CoinCount}";
        if (CoinCount <= 0)
        {
            EmitSignal(SignalName.AllGrabbed);
        }
    }
}
