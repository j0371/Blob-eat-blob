using System;
using BlobEatBlob.HelperScripts;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    //Necessary Godot Game Properties
    protected override bool IsReady => base.IsReady;


    //Node Properties
    private Vector2? _roamingDirection = null;

    private Player _player;

    public bool IsPreSpawning { get; private set; }

    public override bool IsLungeUnlocked => _player.Level >= GameConfig.NonPlayer.LungeUnlockLevel;

    public override bool IsDeflectUnlocked => _player.Level >= GameConfig.NonPlayer.DeflectUnlockLevel;

    protected override float BaseSpeed => !IsInstanceValid(_player)
    ? GameConfig.Blob.Speed
    : (_player.Level == 1 ? GameConfig.NonPlayer.LevelOneSpeed : GameConfig.Blob.Speed) * _player.LevelScaleFactor;

    private double _defendPressSecondsLeft;

    public override bool IsDefendPressed => _defendPressSecondsLeft > 0;


    //Other Properties
    private RandomNumberGenerator randomRoamingGenerator = new();

    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        base._Ready();

        GetNode<Area2D>("DeflectDetection").BodyEntered += OnDeflectDetected;

    }

    public void SetInitialValues(Player player)
    {

        _player = player;

        RandomNumberGenerator randomGrow = new();
        randomGrow.Randomize();

        Size = randomGrow.RandiRange(Math.Max(player.Size - GameConfig.NonPlayer.blobSpawnSizeRangeFromPlayer, 1), player.Size + GameConfig.NonPlayer.blobSpawnSizeRangeFromPlayer);
        UpdateScale();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!IsKnockedBack)
        {
            Move();

            if (_roamingDirection is Vector2 roamingDirection)
                Velocity = roamingDirection * Speed;
        }

        MoveAndSlide();

        _defendPressSecondsLeft -= delta;
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

    private void OnDeflectDetected(Node2D body)
    {
        if (body == this || body is not Blob) return;

        if (randomRoamingGenerator.Randf() < GameConfig.NonPlayer.DeflectChance)
            _defendPressSecondsLeft = GameConfig.NonPlayer.DeflectPressSeconds;
    }
}
