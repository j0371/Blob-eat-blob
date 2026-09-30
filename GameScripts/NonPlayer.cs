using BlobEatBlob.HelperScripts;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    //Necessary Godot Game Properties
    protected override bool IsReady => base.IsReady;


    //Node Properties
    private Vector2? _roamingDirection = null;

    public bool IsPreSpawning { get; private set; }


    //Other Properties
    private RandomNumberGenerator randomRoamingGenerator = new();

    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        base._Ready();

    }

    public void SetInitialSize(Player player)
    {
        RandomNumberGenerator randomGrow = new();
        randomGrow.Randomize();

        Size = randomGrow.RandiRange(-GameConfig.NonPlayer.blobSpawnSizeRangeFromPlayer + player.Size, GameConfig.NonPlayer.blobSpawnSizeRangeFromPlayer + player.Size);
        Scale = _baseScale * Size;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Move();

        if (_roamingDirection is Vector2 roamingDirection)
            Velocity = roamingDirection * Speed;

        MoveAndSlide();
    }

    private void Move()
    {
        Roam();
        //TODO: add "blob detected" movement
    }

    private void Roam()
    {
        if (_roamingDirection is not null && randomRoamingGenerator.RandiRange(0, GameConfig.NonPlayer.DirectionChangeWeight) != 0) return;

        Vector2 newRoamingDirection = Vector2.FromAngle(randomRoamingGenerator.RandiRange(0, 359));
        
        _roamingDirection = newRoamingDirection;
        Velocity = newRoamingDirection * Speed; 
    }

    public bool SetIsPreSpawning(bool isPreSpawning)
    {
        IsPreSpawning = isPreSpawning;
        SetPhysicsProcess(isPreSpawning);
        SetProcess(isPreSpawning);

        return IsPreSpawning;
    }
}
