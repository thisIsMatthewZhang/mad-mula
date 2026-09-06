using Godot;

public partial class Ui : Control
{
    public void InitializeCoinCount(int coinCount)
    {

        RemainingCoins remainingCoinsLabel = GetNode<RemainingCoins>("RemainingCoins");
        remainingCoinsLabel.CoinCount = coinCount;
        remainingCoinsLabel.Text = $"Coins left: {coinCount}";
    }

    public void InitializeCountdown(double initialTime)
    {
        TimerLabel countdownLabel = GetNode<TimerLabel>("TimerLabel");
        countdownLabel.Text = $"Time left: {initialTime}";
    }
}
