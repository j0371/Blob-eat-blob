using BlobEatBlob.Enums;
using Godot;
using System;
using System.Collections.Generic;

namespace BlobEatBlob.Scripts;

public partial class NonPlayerSpawner : Node2D
{

    private const List<Direction> _directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    [Export]
    private CollisionShape2D _activeBounds;

    private readonly PackedScene _nonPlayerScene;

    private readonly RandomNumberGenerator _random = new();

    private readonly int _maxNonPlayerCount = 10;

    private readonly int _nonPlayerCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var ActiveBoundsrectangle = (RectangleShape2D) _activeBounds.Shape;

        Vector2 size = ActiveBoundsrectangle.Size;
        Vector2 center = _activeBounds.GlobalPosition;

        var directions = Enum.GetValues<Direction>();

    }
    
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (_nonPlayerScene is null)
        {
            GD.PushError("NonPlayerScene has not been instantiated.");
            return;
        }

        for (int i = _nonPlayerCount; i < _maxNonPlayerCount; i++)
        {
            Node2D nonPlayer = _nonPlayerScene.Instantiate<Node2D>();

            //Vector2 position = new(
            //    _random.RandfRange(SpawnArea.Position.X, SpawnArea.End.X),
            //    _random.RandfRange(SpawnArea.Position.Y, SpawnArea.End.Y));

            //nonPlayer.Position = position;
            AddChild(nonPlayer);
        }
    }
}
