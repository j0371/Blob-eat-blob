using Blobeatblob.Enums;
using BlobEatBlob.Scripts;
using Godot;
using System;

namespace BlobEatBlob.Scripts;

public partial class Blob : CharacterBody2D
{
    private Area2D _detectionArea;

    private CollisionShape2D _detectionShape;

    private Vector2 _baseScale;

    public int Size { get; private set; } = 1;

    public float Speed { get; private set; } = 300.0f;

    // World-space radius of the area this blob can eat within.
    public float DetectionRadius =>
        ((CircleShape2D)_detectionShape.Shape).Radius * _detectionShape.GlobalScale.X;

    public override void _Ready()
    {
        _baseScale = Scale;

        _detectionArea = GetNode<Area2D>("EnemyDetection"); //TODO: Enum?
        _detectionArea.BodyEntered += OnEnemyTouched;
        _detectionShape = _detectionArea.GetNode<CollisionShape2D>("DetectionRectangle");

        Grow(BlobGrowAmount.Small.Size());
    }

    public override void _PhysicsProcess(double delta)
	{
        
    }

    private void OnEnemyTouched(Node2D body)
    {
        if (body == this || body is not Blob)
        {
            return;
        }

        Blob enemy = body as Blob;

        if (this.Size < enemy.Size)
        {
            QueueFree();
            return;
        }

        EatEnemy(enemy);
    }

    private void EatEnemy(Blob enemy)
    {
        Grow(enemy.Size > this.Size ? BlobGrowAmount.Large.Size() : BlobGrowAmount.Small.Size());
        enemy.QueueFree();
    }

    protected void Grow(int growAmount)
    {
        Size += growAmount;
        Scale = _baseScale * Size;
    }
}
