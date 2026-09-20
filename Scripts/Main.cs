using Godot;
using System;

public partial class Main : Node2D
{
    [Export]
    public PackedScene NonPlayerScene { get; set; }

    [Export]
    public int NonPlayerCount { get; set; } = 10;

    [Export]
    public Rect2 SpawnArea { get; set; } =
        new Rect2(-500, -300, 1000, 600);

    private readonly RandomNumberGenerator _random = new();

    public override void _Ready()
    {
        _random.Randomize();
        SpawnNonPlayers();
    }

    private void SpawnNonPlayers()
    {
        if (NonPlayerScene is null)
        {
            GD.PushError("NonPlayerScene has not been instantiated.");
            return;
        }

        for (int i = 0; i < NonPlayerCount; i++)
        {
            Node2D nonPlayer = NonPlayerScene.Instantiate<Node2D>();

            Vector2 position = new(
                _random.RandfRange(SpawnArea.Position.X, SpawnArea.End.X),
                _random.RandfRange(SpawnArea.Position.Y, SpawnArea.End.Y));

            nonPlayer.Position = position;
            AddChild(nonPlayer);
        }
    }
}
