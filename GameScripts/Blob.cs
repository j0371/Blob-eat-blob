using Blobeatblob.Enums;
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

    public int Size { get; private set; } = 1;

    public float Speed { get; private set; } = 300.0f;

    public float Radius => ((CircleShape2D) GetNode<CollisionShape2D>("PhysicalCollision").Shape).Radius * GlobalScale.X;

    public virtual bool IsAttackPressed => false;

    public virtual bool IsDefendPressed => false;
    public virtual Vector2 AimDirection => Vector2.Zero;


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        _baseScale = Scale;

        _detectionArea.BodyEntered += OnEnemyTouched;
    }

    public override void _PhysicsProcess(double delta)
	{
        
    }

    private void OnEnemyTouched(Node2D body)
    {
        if (body == this || body is not Blob || body is Player)
        {
            return;
        }

        NonPlayer enemy = (NonPlayer) body;

        if (enemy.IsPreSpawning)
        {
            return;
        }

        if (this.Size < enemy.Size)
        {
            QueueFree();
            return;
        }

        EatEnemy(enemy);
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
}
