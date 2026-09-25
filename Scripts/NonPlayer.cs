using BlobEatBlob.Enums;
using BlobEatBlob.Scripts;
using Godot;
using System;
using System.Collections.Generic;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    private Direction _moveDirection;

    public override void _Ready()
    {
        base._Ready();

        RandomNumberGenerator randomGrow = new();
        randomGrow.Randomize();
        Grow(randomGrow.RandiRange(0, 1));

        RandomNumberGenerator random = new();
        random.Randomize();
        _moveDirection = (Direction) random.RandiRange(0, 4);
    }

    public override void _PhysicsProcess(double delta)
	{
		
	}

    private Vector2 Move()
    {
        Velocity = _moveDirection.Vector() * Speed;
        MoveAndSlide();

        return Velocity;
    }
}
