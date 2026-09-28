using System;
using System.Dynamic;
using System.Transactions;
using BlobEatBlob.Enums;
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

        RandomNumberGenerator randomGrow = new();
        randomGrow.Randomize();
        Grow(randomGrow.RandiRange(0, 1));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Roam();

        MoveAndSlide();
    }

    private void Roam()
    {
        if (_roamingDirection is not null && randomRoamingGenerator.RandiRange(0, 300) != 0) return;

        Vector2 newRoamingDirection = Vector2.FromAngle(randomRoamingGenerator.RandiRange(0, 359)); //((Direction)randomRoamingGenerator.RandiRange(0, 7)).Vector();
        
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
