using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }
    [Export]
    public PackedScene StalkerScene { get; set; }

    private int StartingCoinCount = GD.RandRange(10, 20);

    private Ui ui;

    private Player player;

    public override void _Ready()
    {
        InitializeCoinsRandomly();
        ui = GetNode<Ui>("UI");
        ui.InitializeCoinCount(StartingCoinCount);
        GetNode("UI/RemainingCoins").Connect(RemainingCoins.SignalName.AllGrabbed, Callable.From(OnRemainingCoinsAllGrabbed));
        player = GetNode<Player>("Player");
        player.Connect(Player.SignalName.CoinGrabbed, Callable.From(OnCoinGrabbed));
        player.SetPhysicsProcess(false);
        GD.Print(player.Position);
        GetNode("CountdownTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnCountdownTimerFinished));
        GetNode("GameTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnGameTimerFinished));
        GetNode<BoxContainer>("UI/Buttons").Hide();
        GetNode("UI/Buttons/QuitButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnQuitButtonPressed));
        GetNode("UI/Buttons/RetryButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnRetryButtonPressed));
        SpawnStalker();
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
            coin.Position = new Vector3((float)GD.RandRange(-box.Size.X / 2 + 1, box.Size.X / 2 - 1), box.Size.Y + 0.5f, (float)GD.RandRange(-box.Size.Z / 2 + 1, box.Size.Z / 2 - 1));
            AddChild(coin);
        }
    }

    private void OnCoinGrabbed()
    {
        GetNode<AudioStreamPlayer>("SoundEffect").Play();
    }

    private void OnCountdownTimerFinished()
    {
        GetNode<CountdownLabel>("UI/CountdownLabel").Visible = false;
        Timer gameTimer = GetNode<Timer>("GameTimer");
        GetNode<TimerLabel>("UI/TimerLabel").Visible = true;
        gameTimer.Start();
        ui.InitializeCountdown(gameTimer.WaitTime);
        player.SetPhysicsProcess(true);
    }

    private void OnRemainingCoinsAllGrabbed()
    {
        player.SetPhysicsProcess(false);
        SetProcess(false);
        GetNode<Timer>("GameTimer").Stop();
        GetNode<TimerLabel>("UI/TimerLabel").Text = "You won!";
        GetNode<BoxContainer>("UI/Buttons").Show();
    }

    private void OnGameTimerFinished()
    {
        if (GetNode<RemainingCoins>("UI/RemainingCoins").CoinCount > 0)
        {
            player.SetPhysicsProcess(false);
            SetProcess(false);
            GetNode<TimerLabel>("UI/TimerLabel").Text = "You failed:(";
            GetNode<BoxContainer>("UI/Buttons").Show();
        }
    }

    private void OnRetryButtonPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitButtonPressed()
    {
        SceneTree sceneTree = GetTree();
        sceneTree.Root.PropagateNotification((int) NotificationWMCloseRequest); // notify nodes in the scene tree that a window close request is made
        sceneTree.Quit();
    }

    private void SpawnStalker()
    {
        Stalker stalker = StalkerScene.Instantiate<Stalker>();
        stalker.SetPlayer(player);
        // stalker.Position = new Vector3(player.Position.X, player.Position.Y + 2.0f, player.Position.Z);
        AddChild(stalker);
        GD.Print(stalker.Position);
    }
}
