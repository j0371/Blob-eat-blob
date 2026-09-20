using BlobEatBlob.Scripts;
using Godot;
using System;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    public override void _Ready()
    {
        base._Ready();

        RandomNumberGenerator random = new();
        random.Randomize();
        Grow(random.RandiRange(0, 1));
    }

    public override void _PhysicsProcess(double delta)
	{
		
	}
}
