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
    protected Vector2 _baseScale;

    private SceneTreeTimer suppressionTimer;

    //1 = just suppressed, 0 = not suppressed
    public double SuppressionRemainingFraction => BlobState is BlobStates.Suppressed && suppressionTimer is not null
        ? suppressionTimer.TimeLeft / GameConfig.Blob.SuppressionSeconds
        : 0;

    public event System.Action CollidedWhileProtected;

    public event System.Action<Blob> Afflicting;

    public event System.Action<BlobStates> StateChanged;

    public event System.Action Eaten;

    public int Size { get; protected set; }

    public virtual float Speed { get; set; } = GameConfig.Blob.Speed;

    public float Radius => ((CircleShape2D) GetNode<CollisionShape2D>("PhysicalCollision").Shape).Radius * GlobalScale.X;

    public virtual bool IsAttackPressed => false;

    public virtual bool IsDefendPressed => false;
    public virtual Vector2 AimDirection => Vector2.Zero;

    public bool IsMovementLocked { get; private set; } = false;

    private BlobStates blobState = BlobStates.Normal;

    //Every blob state change goes through here, so StateChanged is the one place to react to it
    public BlobStates BlobState
    {
        get => blobState;
        private set
        {
            if (blobState == value) return;

            blobState = value;
            StateChanged?.Invoke(value);
        }
    }

    public AfflictingStates AfflictingState { get; private set; } = AfflictingStates.Normal;

    public virtual bool IsLungeUnlocked => true;

    public virtual bool IsDeflectUnlocked => true;


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        _baseScale = Scale;

        _detectionArea.BodyEntered += OnEnemyCollide;
        Afflicting += OnAfflicted;
        StateChanged += OnStateChanged;
    }

    public override void _PhysicsProcess(double delta)
	{
        
    }

    private void OnEnemyCollide(Node2D body)
    {
        if (body == this || body is not Blob) return;

        Blob enemy = (Blob) body;

        if (enemy.AfflictingState is AfflictingStates.Suppressing) Afflict(enemy);

        if(enemy.BlobState is not (BlobStates.Protected or BlobStates.Shrouded) &&
        BlobState is not BlobStates.Suppressed &&
        Size > enemy.Size || blobState is BlobStates.Protected || enemy.blobState is BlobStates.Suppressed)
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
        enemy.Eaten?.Invoke();
    }

    protected virtual void Grow(int growAmount)
    {
        Size += growAmount;
        UpdateScale();
    }

    public void Suppress()
    {
        BlobState = BlobStates.Suppressed;

        CancelSuppressionTimer();
        suppressionTimer = GetTree().CreateTimer(GameConfig.Blob.SuppressionSeconds);
        suppressionTimer.Timeout += OnSuppressionEnded;
    }

    private void OnSuppressionEnded()
    {
        suppressionTimer = null;
        if (BlobState is BlobStates.Suppressed) BlobState = BlobStates.Normal;
    }

    private void CancelSuppressionTimer()
    {
        if (suppressionTimer is not null) suppressionTimer.Timeout -= OnSuppressionEnded;
        suppressionTimer = null;
    }

    public void ClearSuppression()
    {
        CancelSuppressionTimer();
        if (BlobState is BlobStates.Suppressed) BlobState = BlobStates.Normal;
    }

    private void OnStateChanged(BlobStates newState)
    {
        Speed = newState is BlobStates.Suppressed
            ? GameConfig.Blob.Speed * GameConfig.Blob.SuppressedSpeedFactor
            : GameConfig.Blob.Speed;
    }

    public void SetBlobState(BlobStates blobstate)
    {
        BlobState = blobstate;
    }

    public void SetIsMovementLocked(bool isMovementLocked)
    {
        IsMovementLocked = isMovementLocked;
    }

    public void SetAfflictingState(AfflictingStates afflictingState)
    {
        AfflictingState = afflictingState;
    }

    protected void UpdateScale() => Scale = _baseScale * Size * GameConfig.Blob.BlobScaleFactor;
}
