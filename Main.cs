using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }

    private int StartingCoinCount = GD.RandRange(10, 20);

    private Ui ui;

    private Player player;

    public override void _Ready()
    {
        InitializeCoinsRandomly();
        ui = GetNode<Ui>("UI");
        ui.InitializeCoinCount(StartingCoinCount);
        player = GetNode<Player>("Player");
        player.CoinGrabbed += OnCoinGrabbed;
        player.SetPhysicsProcess(false);
        GetNode<Timer>("CountdownTimer").Timeout += OnCountdownFinished;
    }
    public override void _Process(double delta)
    {   
        double timeLeft = GetNode<Timer>("GameTimer").TimeLeft; // 0.5 is an slight buffer so label UI doesn't start at 29s
        GetNode<TimerLabel>("UI/TimerLabel").UpdateTimeRemaining(double.Truncate(timeLeft));
        double countdownTimeLeft = GetNode<Timer>("CountdownTimer").TimeLeft;
        GetNode<CountdownLabel>("UI/CountdownLabel").DecrementCountdown(double.Truncate(countdownTimeLeft));
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

    private void OnCountdownFinished()
    {
        GetNode<CountdownLabel>("UI/CountdownLabel").QueueFree();
        Timer gameTimer = GetNode<Timer>("GameTimer");
        GetNode<TimerLabel>("UI/TimerLabel").Visible = true;
        gameTimer.Start();
        ui.InitializeCountdown(gameTimer.WaitTime);
        player.SetPhysicsProcess(true);

    }
}
