using System;
using System.Linq;
using Godot;
using MadMula.LevelManager;
using MadMula.RandomNames;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }
    [Export]
    public PackedScene StalkerScene { get; set; }
    [Export]
    public PackedScene PlayerScene { get; set; }
    [Signal]
    public delegate void SpawnedStalkerEventHandler();
    
    private LevelManager levelManager;
    private Stalker _stalker;

    private int _startingCoinCount = GD.RandRange(10, 20);

    private Ui _ui;

    private Player player;

    private Vector3 _playerStartingPosition;

    private bool _stalkerSpawned = false;

    public override void _Ready()
    {
        // levelManager = GetNode<LevelManager>("/root/LevelManager");
        GetNode<RemainingCoins>("UI/RemainingCoins").Hide();
        GetNode<CountdownLabel>("UI/CountdownLabel").Hide();
        GetNode<BoxContainer>("UI/Buttons").Hide();
        SetProcess(false);

        GetNode("/root/Main/LevelContainer").ChildEnteredTree += (node) => {
            InitializeCoinsRandomly();
            _ui = GetNode<Ui>("UI");
            _ui.InitializeCoinCount(_startingCoinCount);
            GetNode<RemainingCoins>("UI/RemainingCoins").Show();
            GetNode<RemainingCoins>("UI/RemainingCoins").Connect(RemainingCoins.SignalName.AllGrabbed, Callable.From(OnRemainingCoinsAllGrabbed));
            GetNode<CountdownLabel>("UI/CountdownLabel").Show();
            player = PlayerScene.Instantiate<Player>();
            player.Connect(Player.SignalName.CoinGrabbed, Callable.From(OnCoinGrabbed));
            player.Connect(Player.SignalName.HitStalker, Callable.From(OnPlayerHitStalker));
            player.Position = new Vector3(0, 5, 0);
            _playerStartingPosition = player.Position;
            AddChild(player);
            player.SetPhysicsProcess(false);
            HBoxContainer hpContainer = GetNode<HBoxContainer>("UI/HPContainer");
            for (int i = 0; i < player.HitPoints; i++)
            {
                ColorRect point = new() {
                    Color = new() { R = 0.79f, G = 0.0f, B = 0.36f, A = 1.0f },
                    CustomMinimumSize = new Vector2(100.0f, 100.0f)
                };
                
                hpContainer.AddChild(point);
            }
            GetNode<Timer>("CountdownTimer").Start();
            GetNode<Timer>("CountdownTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnCountdownTimerFinished));
            GetNode("GameTimer").Connect(Timer.SignalName.Timeout, Callable.From(OnGameTimerFinished));
            GetNode("UI/Buttons/QuitButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnQuitButtonPressed));
            GetNode("UI/Buttons/RetryButton").Connect(BaseButton.SignalName.Pressed, Callable.From(OnRetryButtonPressed));
            SetProcess(true);
            if (!player.DEBUG_PLAYER_MOVEMENT_INFO)
            {
                GetNode<Label>("UI/PlayerMovementInfo").Hide();
            }
        };
    }
    public override void _Process(double delta)
    {   
        GD.Print($"Level: {GetNode("/root/Main/LevelContainer").GetChild<StaticBody3D>(-1).Position}");
        GD.Print($"Player: {player.Position}");
        Timer gameTimer = GetNode<Timer>("GameTimer"); // 0.5 is an slight buffer so label UI doesn't start at 29s
        GetNode<TimerLabel>("UI/TimerLabel").UpdateTimeRemaining(double.Truncate(gameTimer.TimeLeft));
        double countdownTimeLeft = GetNode<Timer>("CountdownTimer").TimeLeft;
        GetNode<CountdownLabel>("UI/CountdownLabel").DecrementCountdown(double.Truncate(countdownTimeLeft));
        if (gameTimer.TimeLeft <= gameTimer.WaitTime / 2.0 && !_stalkerSpawned && GetNode<TimerLabel>("UI/TimerLabel").Visible)
        {
            SpawnStalker();
            _stalkerSpawned = true;
        }
        if (player.DEBUG_PLAYER_MOVEMENT_INFO)
        {
            GetNode<Label>("UI/PlayerMovementInfo").Text = $"Velocity: {player.Velocity}\nUp direction: {player.UpDirection}\nPosition: {player.Position}";
        }
        
    }

    private void InitializeCoinsRandomly()
    {
        var levelContainer = GetNode<Node3D>("LevelContainer");
        Shape3D shape = Determine3DShape(levelContainer.GetChild(0).GetNode<CollisionShape3D>("CollisionShape3D").Shape);
        for (int i = 0; i < _startingCoinCount; i++)
        {
            Coin coin = CoinScene.Instantiate<Coin>();
            coin.Grabbed += GetNode<RemainingCoins>("UI/RemainingCoins").OnCoinGrabbed;
            coin.Position = GiveRandomSpawnPoint(shape);
            AddChild(coin);
        }
    }

    private static Shape3D Determine3DShape(Shape3D shape) => shape switch
    {
        BoxShape3D box => box,
        CylinderShape3D cylinder => cylinder,
        _ => throw new NotImplementedException("The shape you're checking for has not been added as a switch arm.")
    };

    private static Vector3 GiveRandomSpawnPoint(Shape3D shape) => shape switch
    {
        BoxShape3D box => new Vector3((float)GD.RandRange(-box.Size.X / 2 + 1, box.Size.X / 2 - 1), box.Size.Y + 0.5f, (float)GD.RandRange(-box.Size.Z / 2 + 1, box.Size.Z / 2 - 1)),
        CylinderShape3D cylinder => new Vector3((float)GD.RandRange(-cylinder.Radius / 2 + 1, cylinder.Radius / 2 - 1), cylinder.Height + 0.5f, (float)GD.RandRange(-cylinder.Radius / 2 + 1, cylinder.Radius / 2 - 1)),
        _ => Vector3.Zero
    };

    public void OnCoinGrabbed()
    {
        GetNode<AudioStreamPlayer>("SoundEffect").Play();
    }

    public void OnCountdownTimerFinished()
    {
        GetNode<CountdownLabel>("UI/CountdownLabel").Visible = false;
        Timer gameTimer = GetNode<Timer>("GameTimer");
        GetNode<TimerLabel>("UI/TimerLabel").Visible = true;
        gameTimer.Start();
        _ui.InitializeCountdown(gameTimer.WaitTime);
        player.SetPhysicsProcess(true);
    }

    public void OnRemainingCoinsAllGrabbed()
    {
        player.SetPhysicsProcess(false);
        _stalker.SetPhysicsProcess(false);
        SetProcess(false);
        GetNode<Timer>("GameTimer").Stop();
        GetNode<TimerLabel>("UI/TimerLabel").Text = "You won!";
        GetNode<BoxContainer>("UI/Buttons").Show();
    }

    public void OnGameTimerFinished()
    {
        if (GetNode<RemainingCoins>("UI/RemainingCoins").CoinCount > 0)
        {
            player.SetPhysicsProcess(false);
            SetProcess(false);
            GetNode<TimerLabel>("UI/TimerLabel").Text = "You failed:(";
            GetNode<BoxContainer>("UI/Buttons").Show();
        }
    }

    public void OnRetryButtonPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    public void OnQuitButtonPressed()
    {
        SceneTree sceneTree = GetTree();
        sceneTree.Root.PropagateNotification((int) NotificationWMCloseRequest); // notify nodes in the scene tree that a window close request is made
        sceneTree.Quit();
    }

    public void SpawnStalker()
    {
        _stalker = StalkerScene.Instantiate<Stalker>();
        _stalker.SetPlayer(player);
        _stalker.Position = _playerStartingPosition;
        AddChild(_stalker);
    }

    public void OnPlayerHitStalker()
    {
        player.DecrementHitPoints();
        player.EnableIFrames();
        HBoxContainer hpContainer = GetNode<HBoxContainer>("UI/HPContainer");
        hpContainer.RemoveChild(hpContainer.GetChildren().Last());
        if (player.HitPoints == 0)
        {
            player.SetPhysicsProcess(false);
            _stalker.SetPhysicsProcess(false);
            SetProcess(false);
            GetNode<Timer>("GameTimer").Stop();
            GetNode<TimerLabel>("UI/TimerLabel").Text = $"{Names.GiveRandomName()} touched you 🤗";
            GetNode<BoxContainer>("UI/Buttons").Show();   
        }
    }
}
