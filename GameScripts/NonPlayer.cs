using BlobEatBlob.Enums;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    //Necessary Godot Game Properties
    protected override bool IsReady => base.IsReady;


    //Node Properties
    private Direction _moveDirection;

    //While pre-spawning, the blob is removed from physics and doesn't process, so nothing can touch it
    public bool IsPreSpawning
    {
        get => ProcessMode == ProcessModeEnum.Disabled;
        set => ProcessMode = value ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
    }


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

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
        base._PhysicsProcess(delta);

        Move();
    }

    private Vector2 Move()
    {
        Velocity = _moveDirection.Vector() * Speed;
        MoveAndSlide();

        return Velocity;
    }
}
