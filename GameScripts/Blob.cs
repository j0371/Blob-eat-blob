using System.Dynamic;
using Blobeatblob.Enums;
using Blobeatblob.Enums.GameStates;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Blob : CharacterBody2D
{
    //Necessary Godot Game Properties
    [Export]
    private Area2D _detectionArea;

    protected virtual bool IsReady => !this.RequiredGamePropertyNull(_detectionArea);


    //Node Properties
    private Vector2 _baseScale;

    private SceneTreeTimer suppressionTimer;

    public event System.Action CollidedWhileProtected;

    public event System.Action<Blob> Afflicting;

    public int Size { get; private set; } = GameConfig.Blob.StartingSize;

    public float Speed { get; private set; } = GameConfig.Blob.Speed;

    public float Radius => ((CircleShape2D) GetNode<CollisionShape2D>("PhysicalCollision").Shape).Radius * GlobalScale.X;

    public virtual bool IsAttackPressed => false;

    public virtual bool IsDefendPressed => false;
    public virtual Vector2 AimDirection => Vector2.Zero;

    public bool IsMovementLocked { get; private set; } = false;

    public BlobStates BlobState { get; private set; } = BlobStates.Normal;

    public AfflictingStates AfflictingState { get; private set; } = AfflictingStates.Normal;


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        _baseScale = Scale;

        _detectionArea.BodyEntered += OnEnemyCollide;
        Afflicting += OnAfflicted;
    }

    public override void _PhysicsProcess(double delta)
	{
        
    }

    private void OnEnemyCollide(Node2D body)
    {
        if (body == this || body is not Blob) return;

        Blob enemy = (Blob) body;

        if (enemy.AfflictingState is AfflictingStates.Suppressing) Afflict(enemy);

        if (Size > enemy.Size && enemy.BlobState is not (BlobStates.Protected or BlobStates.Shrouded)
        || enemy.BlobState == BlobStates.Suppressed && AfflictingState is not AfflictingStates.Suppressing)
            EatEnemy(enemy);

        if (BlobState is BlobStates.Protected)
        {
            ClearSuppression();
            CollidedWhileProtected?.Invoke();
        }
    }

    public void Afflict(Blob afflicter)
    {
        Afflicting?.Invoke(afflicter);
    }

    protected virtual void OnAfflicted(Blob afflicter)
    {
        if (BlobState is BlobStates.Protected) return;

        Suppress();
    }

    protected virtual void EatEnemy(Blob enemy)
    {
        

        Grow(enemy.Size > this.Size ? BlobGrowAmount.Large.Size() : BlobGrowAmount.Small.Size());
        enemy.QueueFree();
    }

    protected void Grow(int growAmount)
    {
        Size += growAmount;
        Scale = _baseScale * Size;
    }

    public void Suppress()
    {
        EnterSuppressedState();

        CancelSuppressionTimer();
        suppressionTimer = GetTree().CreateTimer(GameConfig.Blob.SuppressionSeconds);
        suppressionTimer.Timeout += OnSuppressionEnded;
    }

    private void OnSuppressionEnded()
    {
        suppressionTimer = null;
        if (BlobState is BlobStates.Suppressed) EnterNormalState();
    }

    private void CancelSuppressionTimer()
    {
        if (suppressionTimer is not null) suppressionTimer.Timeout -= OnSuppressionEnded;
        suppressionTimer = null;
    }

    public void ClearSuppression()
    {
        CancelSuppressionTimer();
        if (BlobState is BlobStates.Suppressed) EnterNormalState();
    }

    protected virtual void EnterNormalState()
    {
        BlobState = BlobStates.Normal;
    }

    protected virtual void EnterSuppressedState()
    {
        BlobState = BlobStates.Suppressed;
    }

    protected void SetSpeed(float speed)
    {
        Speed = speed;
    }

    public void SetBlobState(BlobStates blobstate)
    {
        switch (blobstate)
        {
            case BlobStates.Normal:
                EnterNormalState();
                break;
            case BlobStates.Suppressed:
                EnterSuppressedState();
                break;
            default:
                BlobState = blobstate;
                break;
        }
    }

    public void SetIsMovementLocked(bool isMovementLocked)
    {
        IsMovementLocked = isMovementLocked;
    }

    public void SetAfflictingState(AfflictingStates afflictingState)
    {
        AfflictingState = afflictingState;
    }
}
