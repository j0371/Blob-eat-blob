using Godot;
using System;

public partial class Main : Node2D
{
    [Export]
    public PackedScene EnemyScene { get; set; }

    [Export]
    public int EnemyCount { get; set; } = 10;

    [Export]
    public Rect2 SpawnArea { get; set; } =
        new Rect2(-500, -300, 1000, 600);

    private readonly RandomNumberGenerator _random = new();

    public override void _Ready()
    {
        _random.Randomize();
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (EnemyScene == null)
        {
            GD.PushError("EnemyScene has not been assigned.");
            return;
        }

        for (int i = 0; i < EnemyCount; i++)
        {
            Node2D enemy = EnemyScene.Instantiate<Node2D>();

            Vector2 position = new(
                _random.RandfRange(SpawnArea.Position.X, SpawnArea.End.X),
                _random.RandfRange(SpawnArea.Position.Y, SpawnArea.End.Y));

            enemy.Position = position;
            AddChild(enemy);
        }
    }
}
