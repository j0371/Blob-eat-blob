
using BlobEatBlob.Enums;
using BlobEatBlob.GameScripts;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Player : Blob
{
    //Necessary Godot Game Properties
    [Export]
    public SquarePlayerCamera2d Camera { get; private set; }

    [Export]
    public CollisionShape2D ActiveBounds{ get; private set; }

    protected override bool IsReady => base.IsReady &&
    !(this.RequiredGamePropertyNull(Camera) | this.RequiredGamePropertyNull(ActiveBounds));


    //Node Properties
    public Rect2 ActiveArea
    {
        get
        {
            float scale = Mathf.Max(ActiveBounds.GlobalScale.X, ActiveBounds.GlobalScale.Y);
            float side = ((RectangleShape2D)ActiveBounds.Shape).Size.X * scale;

            return new Rect2(ActiveBounds.GlobalPosition - Vector2.One * side / 2, Vector2.One * side);
        }
    }

    public override bool IsAttackPressed => Input.IsActionPressed(GameConfig.InputActions.Attack);
    public override bool IsDefendPressed => Input.IsActionPressed(GameConfig.InputActions.Defend);
    public override Vector2 AimDirection => (GetGlobalMousePosition() - GlobalPosition).Normalized();

    public int Level => (Size - GameConfig.Blob.StartingSize) / GameConfig.Player.IncreasedSizeToLevelUp + GameConfig.Player.StartingLevel;

    public override bool IsLungeUnlocked => Level >= GameConfig.Player.LungeUnlockLevel;

    public override bool IsDeflectUnlocked => Level >= GameConfig.Player.DeflectUnlockLevel;

    public float LevelProgress =>
    (float)((Size - GameConfig.Blob.StartingSize) % GameConfig.Player.IncreasedSizeToLevelUp) / GameConfig.Player.IncreasedSizeToLevelUp;

    public event System.Action<int> LeveledUp;


    //Other Properties


    //Methods

    public override void _Ready()
    {
        if (!IsReady) return;

        base._Ready();

        Size = GameConfig.Blob.StartingSize;
        UpdateScale();

        RectangleShape2D activeSquare = (RectangleShape2D)ActiveBounds.Shape;

        if (GeometryWarning.RectangleIsNotASquareWarning(activeSquare))
        {
            activeSquare.Size = Vector2.One * Mathf.Max(activeSquare.Size.X, activeSquare.Size.Y);
        }
	}

    public override void _PhysicsProcess(double delta)
    {

        base._PhysicsProcess(delta);

        Move(delta);

        MoveAndSlide();

    }

    private void Move(double delta)
    {
        if (IsMovementLocked) return;

        Vector2 direction = Input.GetVector(
            Direction.Left.Input(),
            Direction.Right.Input(),
            Direction.Up.Input(),
            Direction.Down.Input());

        float rate = direction == Vector2.Zero ? GameConfig.Blob.Deceleration : GameConfig.Blob.Acceleration;

        Velocity = Velocity.MoveToward(direction * Speed, rate * (float)delta);
    }

    protected override void Grow(int growAmount)
    {
        int previousLevel = Level;
        base.Grow(growAmount);

        if (Level > previousLevel) LeveledUp?.Invoke(Level);
    }
}
