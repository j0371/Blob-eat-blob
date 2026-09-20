using BlobEatBlob.Scripts;
using Godot;
using System;

namespace BlobEatBlob.Scripts;

public partial class Blob : CharacterBody2D
{
	public float Speed = 300.0f;

    private Area2D _detectionArea;

    public override void _Ready()
    {
        _detectionArea = GetNode<Area2D>("EnemyDetection"); //TODO: Enum?
        _detectionArea.BodyEntered += OnEnemyTouched;
    }

    public override void _PhysicsProcess(double delta)
	{

    }

    private void OnEnemyTouched(Node2D body)
    {
        if (body is Blob enemy && enemy != this)
        {
            enemy.QueueFree();
        }
    }
}
