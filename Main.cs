using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }
    [Export]
    public PackedScene StalkerScene { get; set; }

    private int _startingCoinCount = GD.RandRange(10, 20);

    private Ui _ui;

    private Player _player;

    private Vector3 _playerStartingPosition;

    private bool _stalkerSpawned = false;

    public override void _Ready()
    {
        InitializeCoinsRandomly();
        _ui = GetNode<Ui>("UI");
        _ui.InitializeCoinCount(_startingCoinCount);
        GetNode("UI/RemainingCoins").Connect(RemainingCoins.SignalName.AllGrabbed, Callable.From(OnRemainingCoinsAllGrabbed));
        _player = GetNode<Player>("Player");
        _player.Connect(Player.SignalName.CoinGrabbed, Callable.From(OnCoinGrabbed));
        _player.SetPhysicsProcess(false);
        _playerStartingPosition = _player.Position;
        GetNode("CountdownTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnCountdownTimerFinished));
        GetNode("GameTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnGameTimerFinished));
        GetNode<BoxContainer>("UI/Buttons").Hide();
        GetNode("UI/Buttons/QuitButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnQuitButtonPressed));
        GetNode("UI/Buttons/RetryButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnRetryButtonPressed));
    }
    public override void _Process(double delta)
    {   
        double timeLeft = GetNode<Timer>("GameTimer").TimeLeft; // 0.5 is an slight buffer so label UI doesn't start at 29s
        GetNode<TimerLabel>("UI/TimerLabel").UpdateTimeRemaining(double.Truncate(timeLeft));
        double countdownTimeLeft = GetNode<Timer>("CountdownTimer").TimeLeft;
        GetNode<CountdownLabel>("UI/CountdownLabel").DecrementCountdown(double.Truncate(countdownTimeLeft));
        if (timeLeft <= 15.0 && !_stalkerSpawned && GetNode<TimerLabel>("UI/TimerLabel").Visible)
        {
            SpawnStalker();
            _stalkerSpawned = true;
        }
    }

    private void InitializeCoinsRandomly()
    {
        BoxShape3D box = (BoxShape3D) GetNode<CollisionShape3D>("Ground/CollisionShape3D").Shape;
        for (int i = 0; i < _startingCoinCount; i++)
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
        _ui.InitializeCountdown(gameTimer.WaitTime);
        _player.SetPhysicsProcess(true);
    }

    private void OnRemainingCoinsAllGrabbed()
    {
        _player.SetPhysicsProcess(false);
        SetProcess(false);
        GetNode<Timer>("GameTimer").Stop();
        GetNode<TimerLabel>("UI/TimerLabel").Text = "You won!";
        GetNode<BoxContainer>("UI/Buttons").Show();
    }

    private void OnGameTimerFinished()
    {
        if (GetNode<RemainingCoins>("UI/RemainingCoins").CoinCount > 0)
        {
            _player.SetPhysicsProcess(false);
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
        stalker.SetPlayer(_player);
        stalker.Position = _playerStartingPosition;
        AddChild(stalker);
    }
}
