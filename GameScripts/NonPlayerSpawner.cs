using Godot;
using System.Collections.Generic;
using System.Linq;
using BlobEatBlob.HelperScripts;
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

        FreeOutOfBoundNonplayers();

        if (SpawnedNonPlayers.Count() >= GameConfig.NonPlayerSpawner.MaxNonPlayerCount || !IsInstanceValid(_player)) return;


        //Getting the outer bounds of the spawn area
        Rect2 spawnAreaOuterBounds = _player.ActiveArea;

        //Getting the inner bounds of the spawn area
        float margin = _player.Radius * GameConfig.NonPlayerSpawner.MarginMultiplier;

        Rect2 spawnAreaInnerBounds = _player.Camera.ViewSquare.Grow(margin);

        if (GeometryError.RectNotEnclosedError(spawnAreaInnerBounds, spawnAreaOuterBounds)) return;


        Rect2[] spawnAreas =
                            [
                                new(spawnAreaOuterBounds.Position.X, spawnAreaOuterBounds.Position.Y, spawnAreaOuterBounds.Size.X, spawnAreaInnerBounds.Position.Y - spawnAreaOuterBounds.Position.Y), // top
                                new(spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.End.Y, spawnAreaOuterBounds.Size.X, spawnAreaOuterBounds.End.Y - spawnAreaInnerBounds.End.Y),               // bottom
                                new(spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.Position.Y, spawnAreaInnerBounds.Position.X - spawnAreaOuterBounds.Position.X, spawnAreaInnerBounds.Size.Y), // left
                                new(spawnAreaInnerBounds.End.X, spawnAreaInnerBounds.Position.Y, spawnAreaOuterBounds.End.X - spawnAreaInnerBounds.End.X, spawnAreaInnerBounds.Size.Y),               // right
                            ];

        //creating new non-player blob
        NonPlayer spawningNonPlayer = _nonPlayerScene.Instantiate<NonPlayer>();

        //Placing the blob at a random point within the spawn areas
        spawningNonPlayer.GlobalPosition = GetRandomPointInAreas(spawnAreas);
        spawningNonPlayer.SetIsPreSpawning(false);

        AddChild(spawningNonPlayer);
        spawningNonPlayer.SetInitialValues(_player);
    }

    private void FreeOutOfBoundNonplayers()
    {
        if (!IsInstanceValid(_player)) return;

        Rect2 spawnAreaOuterBounds = _player.ActiveArea;

        //Despawning blobs that wandered out of the active area
        foreach (NonPlayer nonPlayer in SpawnedNonPlayers)
            if (!spawnAreaOuterBounds.HasPoint(nonPlayer.GlobalPosition)) nonPlayer.QueueFree();

        if (SpawnedNonPlayers.Count() >= GameConfig.NonPlayerSpawner.MaxNonPlayerCount) return;
    }

    private Vector2 GetRandomPointInAreas(Rect2[] areas)
    {
        //Picking an area weighted by its size, so spawns are evenly spread across all areas
        float roll = _random.RandfRange(0, areas.Sum(area => area.Area));
        Rect2 chosenArea = areas[^1];

        foreach (Rect2 area in areas)
        {
            if (roll <= area.Area)
            {
                chosenArea = area;
                break;
            }

            roll -= area.Area;
        }

        return new Vector2(
            _random.RandfRange(chosenArea.Position.X, chosenArea.End.X),
            _random.RandfRange(chosenArea.Position.Y, chosenArea.End.Y));
    }

}