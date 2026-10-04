
using BlobEatBlob.Enums;
using BlobEatBlob.GameScripts;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Player : Blob
{

    private static int respawnLevel = GameConfig.Player.StartingLevel;

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

    public int LevelStartSize => GameConfig.Blob.StartingSize + (Level - GameConfig.Player.StartingLevel) * GameConfig.Player.IncreasedSizeToLevelUp;

    //how far the camera has zoomed out; world speeds are multiplied by this so on-screen speed stays the same
    public float LevelScaleFactor => (float)LevelStartSize / GameConfig.Blob.StartingSize;

    protected override float BaseSpeed => GameConfig.Blob.Speed * LevelScaleFactor;


    //Other Properties


    //Methods

    public override void _Ready()
    {
        if (!IsReady) return;

        base._Ready();

        //start at 0 progress of the level the player died at
        Size = GameConfig.Blob.StartingSize + (respawnLevel - GameConfig.Player.StartingLevel) * GameConfig.Player.IncreasedSizeToLevelUp;

        Eaten += OnEaten;

        UpdateScale();

        RectangleShape2D activeSquare = (RectangleShape2D)ActiveBounds.Shape;

        if (GeometryWarning.RectangleIsNotASquareWarning(activeSquare))
        {
            activeSquare.Size = Vector2.One * Mathf.Max(activeSquare.Size.X, activeSquare.Size.Y);
        }

        Camera.NormalizeZoom(LevelStartSize);
        LeveledUp += NormalizeCameraZoom;
    }

    public override void _PhysicsProcess(double delta)
    {

        base._PhysicsProcess(delta);

        Move(delta);

        MoveAndSlide();

    }

    private void Move(double delta)
    {
        if (IsMovementLocked || IsKnockedBack) return;

        Vector2 direction = Input.GetVector(
            Direction.Left.Input(),
            Direction.Right.Input(),
            Direction.Up.Input(),
            Direction.Down.Input());

        float rate = (direction == Vector2.Zero ? GameConfig.Blob.Deceleration : GameConfig.Blob.Acceleration) * LevelScaleFactor;

        Velocity = Velocity.MoveToward(direction * Speed, rate * (float)delta);
    }

    protected override void Grow(int growAmount)
    {
        int previousLevel = Level;
        base.Grow(growAmount);

        if (Level > previousLevel) LeveledUp?.Invoke(Level);
    }

    private void OnEaten()
    {
        respawnLevel = Level;
    }

    private void NormalizeCameraZoom(int _)
    {
        Camera.NormalizeZoom(LevelStartSize);
    }
}
