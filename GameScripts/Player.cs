
using BlobEatBlob.Enums;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Player : Blob
{

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
