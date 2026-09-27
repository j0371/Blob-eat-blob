using System.Dynamic;
using BlobEatBlob.Enums;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    //Necessary Godot Game Properties
    protected override bool IsReady => base.IsReady;


    //Node Properties
    private Direction _moveDirection;

    public bool IsPreSpawning { get; private set; }

    //Other Properties

    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        base._Ready();

        RandomNumberGenerator randomGrow = new();
        randomGrow.Randomize();
        Grow(randomGrow.RandiRange(0, 1));

        MoveAndSlide(); //Move();
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        MoveAndSlide(); //Move();
    }

    private void Move()
    {
        RandomNumberGenerator random = new();
        random.Randomize();
        _moveDirection = (Direction)random.RandiRange(0, 3);
        return;
    }

    public bool SetIsPreSpawning(bool isPreSpawning)
    {
        IsPreSpawning = isPreSpawning;
        SetPhysicsProcess(isPreSpawning);
        SetProcess(isPreSpawning);

        return IsPreSpawning;
    }
}
