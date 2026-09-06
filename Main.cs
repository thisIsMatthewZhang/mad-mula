using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }

    private int StartingCoinCount = GD.RandRange(10, 20);

    public override void _Ready()
    {
        InitializeCoinsRandomly();
        Ui ui = GetNode<Ui>("UI");
        ui.InitializeCoinCount(StartingCoinCount);
        ui.InitializeCountdown(GetNode<Timer>("GameTimer").WaitTime);
        GetNode<Player>("Player").CoinGrabbed += OnCoinGrabbed;
    }
    public override void _Process(double delta)
    {   
        double timeLeft = GetNode<Timer>("GameTimer").TimeLeft + 0.5; // 0.5 is an slight buffer so label UI doesn't start at 29s
        GetNode<CountdownLabel>("UI/TimerLabel").UpdateTimeRemaining(double.Truncate(timeLeft));
    }

    private void InitializeCoinsRandomly()
    {
        BoxShape3D box = (BoxShape3D) GetNode<CollisionShape3D>("Ground/CollisionShape3D").Shape;
        for (int i = 0; i < StartingCoinCount; i++)
        {
            // TODO: logic to randomly lay coins around the level
            Coin coin = CoinScene.Instantiate<Coin>();
            coin.Grabbed += GetNode<RemainingCoins>("UI/RemainingCoins").OnCoinGrabbed;
            coin.Position = new Vector3((float)GD.RandRange(-box.Size.X / 2 + 1, box.Size.X / 2 - 1), box.Size.Y + 1.0f, (float)GD.RandRange(-box.Size.Z / 2 + 1, box.Size.Z / 2 - 1));
            AddChild(coin);
        }
    }

    private void OnCoinGrabbed()
    {
        GetNode<AudioStreamPlayer>("SoundEffect").Play();
    }
}
