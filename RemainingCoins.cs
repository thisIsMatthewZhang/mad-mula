using Godot;

public partial class RemainingCoins : Label
{
    public int CoinCount { get; set; }

    public void OnCoinGrabbed()
    {
        Text = $"Coins left: {--CoinCount}";
    }
}
