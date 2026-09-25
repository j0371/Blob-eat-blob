using BlobEatBlob.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlobEatBlob.Scripts;

public partial class NonPlayerSpawner : Node2D
{

    [Export]
    private CollisionShape2D _activeBounds;

    private Rect2 _spawnArea;

    private readonly PackedScene _nonPlayerScene;

    private readonly RandomNumberGenerator _random = new();

    private readonly int _maxNonPlayerCount = 10;

    private int _nonPlayerCount = 0;

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		List<Direction> directions = [.. Enum.GetValues<Direction>()];

		RectangleShape2D activeBoundsRectangle = (RectangleShape2D)_activeBounds.Shape;

		Vector2 size = activeBoundsRectangle.Size;
		Vector2 center = _activeBounds.GlobalPosition;

		_spawnArea = new(center - size / 2, size);

	}
    
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if(_nonPlayerScene is null)
        {
            GD.PushError("NonPlayerScene has not been instantiated.");
            throw new NullReferenceException();
        }

        if(_nonPlayerCount < _maxNonPlayerCount)
        {
            Node2D nonPlayer = _nonPlayerScene.Instantiate<Node2D>();

            nonPlayer.Position = GetSpawnPosition();
            AddChild(nonPlayer);
            _nonPlayerCount++;
        }
    }

    private Vector2 GetSpawnPosition()
    {
        Rect2? visibleRect = GetCameraVisibleRect();
        Vector2 position;

        if (visibleRect.Value.HasPoint(position))
        {
            position = new Vector2(
                _random.RandfRange(_spawnArea.Position.X, _spawnArea.End.X),
                _random.RandfRange(_spawnArea.Position.Y, _spawnArea.End.Y));

        }

        return position;
    }

    private Rect2? GetCameraVisibleRect()
    {
        Camera2D playerCamera = GetViewport().GetCamera2D();

        if (playerCamera is null)
        {
            GD.PushError("Player camera has not been instantiated.");
            throw new NullReferenceException();
        }

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 halfExtents = viewportSize / 2 / playerCamera.Zoom;

        return new Rect2(playerCamera.GetScreenCenterPosition() - halfExtents, halfExtents * 2);
    }
}
