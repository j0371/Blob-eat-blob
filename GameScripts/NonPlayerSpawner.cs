using BlobEatBlob.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.Scripts;

namespace BlobEatBlob.GameScripts;

public partial class NonPlayerSpawner : Node2D
{
    //Necessary Godot Game Properties
    [Export]
    private CollisionShape2D _activeBounds;

    private readonly PackedScene _nonPlayerScene;

    private bool IsReady => !(this.DisableIfMissing(_nonPlayerScene) | this.DisableIfMissing(_activeBounds));



    //Node Properties
    private Rect2 _spawnArea;

    public IEnumerable<NonPlayer> SpawnedNonPlayers => GetChildren().OfType<NonPlayer>();

    private const int MaxNonPlayerCount = 10;


    //Other Properties
    private readonly RandomNumberGenerator _random = new();

    public override void _Ready()
    {
        if (!IsReady) return;

        List<Direction> directions = [.. Enum.GetValues<Direction>()];

		RectangleShape2D activeBoundsRectangle = (RectangleShape2D)_activeBounds.Shape;

		Vector2 size = activeBoundsRectangle.Size;
		Vector2 center = _activeBounds.GlobalPosition;

		_spawnArea = new(center - size / 2, size);

	}
    
	public override void _Process(double delta)
    {
        if (SpawnedNonPlayers.Count() >= MaxNonPlayerCount) return;

        Node2D nonPlayer = _nonPlayerScene.Instantiate<Node2D>();

        nonPlayer.Position = GetSpawnPosition();
        AddChild(nonPlayer);
    }

    private Vector2 GetSpawnPosition()
    {
        Rect2? visibleRect = GetSpawnAreaInnerBounds();
        Vector2 position = new Vector2();

        if (visibleRect.Value.HasPoint(position))
        {
            position = new Vector2(
                _random.RandfRange(_spawnArea.Position.X, _spawnArea.End.X),
                _random.RandfRange(_spawnArea.Position.Y, _spawnArea.End.Y));

        }

        return position;
    }

    private IEnumerable<Rect2> GetSpawnAreas()
    {
        Rect2 innerBounds = GetSpawnAreaInnerBounds();
        return [innerBounds];
    }

    private Rect2 GetSpawnAreaInnerBounds ()
    {
        Camera2D playerCamera = GetViewport().GetCamera2D();

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 halfExtents = viewportSize / 2 / playerCamera.Zoom;

        return new Rect2(playerCamera.GetScreenCenterPosition() - halfExtents, halfExtents * 2);
    }
}