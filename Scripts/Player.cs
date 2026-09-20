
using BlobEatBlob.Enums;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Player : Blob
{

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector(
            Direction.Left.Input(),
            Direction.Right.Input(),
            Direction.Up.Input(),
            Direction.Down.Input());

        Velocity = direction * Speed;
        MoveAndSlide();
    }
}
