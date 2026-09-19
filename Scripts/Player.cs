
using Godot;
using System;

namespace BlobEatBlob.Scripts;

public partial class Player : CharacterBody2D
{
    public const float Speed = 300.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("up"))
            direction += Vector2.Up;
        else if (Input.IsActionPressed("down"))
            direction += Vector2.Down;
        else if (Input.IsActionPressed("left"))
            direction += Vector2.Left;
        else if (Input.IsActionPressed("right"))
            direction += Vector2.Right;

        Velocity = direction * Speed;
        MoveAndSlide();
    }
}
