
using BlobEatBlob.Enums;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Player : Blob
{
    //Necessary Godot Game Properties
    [Export]
    public Camera2D Camera { get; private set; }

    [Export]
    public CollisionShape2D ActiveBounds { get; private set; }

    protected override bool IsReady => base.IsReady &&
    !(this.RequiredGamePropertyNull(Camera) | this.RequiredGamePropertyNull(ActiveBounds));


    //Node Properties


    //Other Properties


    //Methods
    public override void _PhysicsProcess(double delta)
    {

        base._PhysicsProcess(delta);

        Move();

    }

    private Vector2 Move()
    {
        Vector2 direction = Input.GetVector(
            Direction.Left.Input(),
            Direction.Right.Input(),
            Direction.Up.Input(),
            Direction.Down.Input());

        Velocity = direction * Speed;
        MoveAndSlide();

        return Velocity;
    }
}
