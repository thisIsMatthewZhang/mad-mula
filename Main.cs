using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene CoinScene { get; set; }

    private readonly int CoinCount = GD.RandRange(10, 20);

    public override void _Ready()
    {
        for (int i = 0; i < CoinCount; i++)
        {
            // TODO: logic to randomly lay coins around the level
        }
    }

    public void OnCoinGrabbed()
    {

    }
}
