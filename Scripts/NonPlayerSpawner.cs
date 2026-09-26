using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlobEatBlob.Scripts;

public partial class NonPlayerSpawner : Node2D
{
    [Export]
    private CollisionShape2D _activeBounds;

    [Export]
    private PackedScene _nonPlayerScene;

    [Export]
    private Blob _player;

    [Export]
    private int _maxNonPlayerCount = 10;

    // Seconds between spawns while below the max.
    [Export]
    private float _spawnInterval = 0.25f;

    // Extra distance outside the camera view that is still treated as "visible",
    // so blobs don't pop in right at the screen edge.
    [Export]
    private float _offscreenMargin = 64f;

    private const int MaxSpawnAttempts = 30;

    private readonly RandomNumberGenerator _random = new();

    private float _spawnCooldown;

    public override void _Ready()
    {
        _random.Randomize();

        if (_activeBounds?.Shape is not RectangleShape2D)
        {
            GD.PushError($"{nameof(NonPlayerSpawner)}: active bounds must be a CollisionShape2D with a RectangleShape2D.");
            SetProcess(false);
            return;
        }

        if (_nonPlayerScene is null)
        {
            GD.PushError($"{nameof(NonPlayerSpawner)}: NonPlayer scene has not been assigned.");
            SetProcess(false);
        }
    }

    public override void _Process(double delta)
    {
        // The bounds (and camera) are freed along with the Player when it gets eaten.
        if (!IsInstanceValid(_activeBounds))
        {
            return;
        }

        Rect2 activeArea = GetActiveArea();

        DespawnOutOfBounds(activeArea);

        _spawnCooldown -= (float)delta;

        if (_spawnCooldown > 0f || GetLiveNonPlayers().Count() >= _maxNonPlayerCount)
        {
            return;
        }

        if (TryGetSpawnPosition(activeArea, out Vector2 spawnPosition))
        {
            // Position before AddChild so the blob never exists in physics at the spawner's origin.
            NonPlayer nonPlayer = _nonPlayerScene.Instantiate<NonPlayer>();
            nonPlayer.Position = ToLocal(spawnPosition);
            AddChild(nonPlayer);
        }

        _spawnCooldown = _spawnInterval;
    }

    private void DespawnOutOfBounds(Rect2 activeArea)
    {
        foreach (NonPlayer nonPlayer in GetLiveNonPlayers())
        {
            if (!activeArea.HasPoint(nonPlayer.GlobalPosition))
            {
                nonPlayer.QueueFree();
            }
        }
    }

    // Blobs that were eaten are queued for deletion but stay children until the end of the frame.
    private IEnumerable<NonPlayer> GetLiveNonPlayers()
    {
        return GetChildren().OfType<NonPlayer>().Where(nonPlayer => !nonPlayer.IsQueuedForDeletion());
    }

    private bool TryGetSpawnPosition(Rect2 activeArea, out Vector2 position)
    {
        Rect2 visibleArea = GetCameraVisibleRect().Grow(_offscreenMargin);

        for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
        {
            position = new Vector2(
                _random.RandfRange(activeArea.Position.X, activeArea.End.X),
                _random.RandfRange(activeArea.Position.Y, activeArea.End.Y));

            if (!visibleArea.HasPoint(position) && !IsWithinPlayerReach(position))
            {
                return true;
            }
        }

        position = Vector2.Zero;
        return false;
    }

    // Once the player is large, its eat radius can reach past the camera edge.
    private bool IsWithinPlayerReach(Vector2 position)
    {
        return IsInstanceValid(_player)
            && position.DistanceTo(_player.GlobalPosition) < _player.DetectionRadius + _offscreenMargin;
    }

    // The bounds are a child of the Player, so this is recalculated every frame to follow it.
    private Rect2 GetActiveArea()
    {
        Vector2 size = ((RectangleShape2D)_activeBounds.Shape).Size * _activeBounds.GlobalScale.Abs();

        return new Rect2(_activeBounds.GlobalPosition - size / 2, size);
    }

    private Rect2 GetCameraVisibleRect()
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
