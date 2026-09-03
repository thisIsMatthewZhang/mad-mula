using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }

    private int StartingCoinCount = GD.RandRange(10, 20);

    public override void _Ready()
    {
        InitializeCoinsRandomly();
        GetNode<Ui>("UI").InitializeCoinCount(StartingCoinCount);
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
}
