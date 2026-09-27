using BlobEatBlob.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using BlobEatBlob.Scripts;

namespace BlobEatBlob.GameScripts;

public partial class NonPlayerSpawner : Node2D
{
    //Necessary Godot Game Properties
    [Export]
    private Player _player;

    [Export]
    private PackedScene _nonPlayerScene;

    protected bool IsReady => !(this.RequiredGamePropertyNull(_player) | this.RequiredGamePropertyNull(_nonPlayerScene));



    //Node Properties
    [Export(PropertyHint.Range, "0,100")]
    private int _maxNonPlayerCount = 10;

    [Export(PropertyHint.Range, "-20,20")]
    private float _marginMultiplier = 1.5f;

    public IEnumerable<NonPlayer> SpawnedNonPlayers => GetChildren().OfType<NonPlayer>();


    //Other Properties
    private readonly RandomNumberGenerator _random = new();


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
	}
    
	public override void _Process(double delta)
    {
        if (SpawnedNonPlayers.Count() >= _maxNonPlayerCount || !IsInstanceValid(_player)) return;

        //creating new non-player blob
        NonPlayer spawningNonPlayer = _nonPlayerScene.Instantiate<NonPlayer>();
        AddChild(spawningNonPlayer);

        //Getting the outer bounds of the spawn area
		Vector2 size = ((RectangleShape2D) _player.ActiveBounds.Shape).Size;
		Vector2 center = _player.ActiveBounds.GlobalPosition;

		Rect2 spawnAreaOuterBounds = new(center - size / 2, size);

        //Getting the inner bounds of the spawn area
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 halfExtents = viewportSize / 2 / _player.Camera.Zoom;

        Rect2 viewableArea = new(_player.Camera.GetScreenCenterPosition() - halfExtents, halfExtents * 2);
        float margin = spawningNonPlayer.Radius * _marginMultiplier;

        Rect2 spawnAreaInnerBounds = viewableArea.Grow(margin);

        if (GeometryError.RectNotEnclosedError(spawnAreaInnerBounds, spawnAreaOuterBounds)) return;

        Rect2[] spawnAreas =
                            [
                                new(spawnAreaOuterBounds.Position.X, spawnAreaOuterBounds.Position.Y, spawnAreaOuterBounds.Size.X, spawnAreaInnerBounds.Position.Y - spawnAreaOuterBounds.Position.Y), // top
                                new(spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.End.Y, spawnAreaOuterBounds.Size.X, spawnAreaOuterBounds.End.Y - spawnAreaInnerBounds.End.Y),               // bottom
                                new(spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.Position.Y, spawnAreaInnerBounds.Position.X - spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.Size.Y), // left
                                new(spawnAreaInnerBounds.End.X, spawnAreaInnerBounds.Position.Y, spawnAreaOuterBounds.End.X - spawnAreaInnerBounds.End.X, spawnAreaInnerBounds.Size.Y),               // right
                            ];
    }

}