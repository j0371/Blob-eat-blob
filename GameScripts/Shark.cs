using Blobeatblob.Enums.GameStates;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.Scripts;
using Godot;
using System;

namespace BlobEatBlob.GameScripts;

public partial class Shark : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private LungeStates LungeState = LungeStates.AttackReady;
    
    private DeflectStates DeflectState = DeflectStates.DeflectReady;

    private AfflictingStates AfflictingState => (LungeState is LungeStates.Lunging || DeflectState is DeflectStates.Deflecting) 
    ? AfflictingStates.Suppressing
    : AfflictingStates.Normal;

    private double CurrentLungeStateTimer;

    private bool lungeHitEnemy;

    private SceneTreeTimer lungeTimer;

    private double CurrentDeflectStateTimer = GameConfig.Shark.DeflectWindowSeconds;

    private Blob SharkBlob => GetParent<Blob>();

    //0 = attack ready, 1 = attack just used
    public double CooldownRemainingFraction => LungeState switch
    {
        LungeStates.Lunging or LungeStates.LungeRecovery => 1,
        LungeStates.OnCooldown => lungeTimer is null ? 0 : lungeTimer.TimeLeft / GameConfig.Shark.AttackCooldownSeconds,
        _ => 0
    };

    public bool IsChargingLunge => LungeState == LungeStates.LungeCharging;

    //0 = no charge, 1 = fully charged
    public double ChargeFraction => IsChargingLunge ? Math.Min(CurrentLungeStateTimer / GameConfig.Shark.MaxChargeSeconds, 1) : 0;

    //true when nothing is blocking a lunge charge from starting
    public bool CanStartLunge => LungeState is LungeStates.AttackReady
        && SharkBlob.BlobState != BlobStates.Suppressed
        && DeflectState is DeflectStates.DeflectReady;

    public bool IsDeflecting => DeflectState == DeflectStates.Deflecting;

    //0 = deflect ready, 1 = deflect just used
    public double DeflectCooldownRemainingFraction => DeflectState switch
    {
        DeflectStates.Deflecting => 1,
        DeflectStates.OnCooldown => Math.Max(CurrentDeflectStateTimer / GameConfig.Shark.DefendCooldownSeconds, 0),
        _ => 0
    };


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        SharkBlob.CollidedWhileProtected += OnBlobCollidedWhileProtected;
    }

    public override void _PhysicsProcess(double delta)
    {
        ManageAttackState(delta);
        ManageDefendState(delta);
        ManageAfflictingState();
    }

#region Attack state management methods

    private void ManageAttackState(double delta)
    {
        switch (LungeState)
        {
            case LungeStates.AttackReady:
                if (SharkBlob.IsAttackPressed && CanStartLunge) EnterLungeChargingState();
                break;

            case LungeStates.LungeCharging:
                CurrentLungeStateTimer += delta;
                bool IsLungeCharged = CurrentLungeStateTimer >= GameConfig.Shark.MaxChargeSeconds || !SharkBlob.IsAttackPressed;
                if (IsLungeCharged) EnterLungingState();
                break;

            case LungeStates.LungeRecovery:
                SharkBlob.Velocity = SharkBlob.Velocity.MoveToward(Vector2.Zero, GameConfig.Blob.Deceleration * (float)delta);
                break;
        }
    }

    private void EnterLungeState(LungeStates state, double seconds = 0, Action onTimeout = null)
    {
        LungeState = state;
        CurrentLungeStateTimer = 0;
        lungeTimer = null; //drops any pending timer: its callback sees a different timer and does nothing

        if (onTimeout is null) return;

        SceneTreeTimer timer = GetTree().CreateTimer(seconds);
        lungeTimer = timer;
        timer.Timeout += () =>
        {
            if (!IsInstanceValid(this) || lungeTimer != timer) return;
            onTimeout();
        };
    }

    private void EnterLungeChargingState()
    {
        EnterLungeState(LungeStates.LungeCharging, 0);
    }

    private void EnterLungingState()
    {
        lungeHitEnemy = false;
        SharkBlob.SetBlobState(BlobStates.Protected);
        SharkBlob.Velocity = SharkBlob.AimDirection * (GameConfig.Blob.Speed * GameConfig.Shark.LungeSpeedMultiplier);
        SharkBlob.SetIsMovementLocked(true);

        double LungingTimer = Math.Min(CurrentLungeStateTimer, GameConfig.Shark.MaxChargeSeconds) * GameConfig.Shark.lungeSecondsPerChargeSecond;
        EnterLungeState(LungeStates.Lunging, LungingTimer, OnLungeEnded);
    }

    private void OnLungeEnded()
    {
        if (lungeHitEnemy) RefreshLungeAbility();
        else EnterLungeRecoveryState();
    }

    private void EnterLungeRecoveryState()
    {
        SharkBlob.SetBlobState(BlobStates.Normal); //ends the lunge Protected state so the self-affliction below is not skipped
        SharkBlob.Afflict(SharkBlob);
        EnterLungeState(LungeStates.LungeRecovery, GameConfig.Shark.LungeRecoverySeconds, EnterLungeCooldownState);
    }

    private void EnterLungeCooldownState()
    {
        //no reset to Normal here, so a missed lunge's suppression runs its full length
        SharkBlob.SetIsMovementLocked(false);
        EnterLungeState(LungeStates.OnCooldown, GameConfig.Shark.AttackCooldownSeconds, EnterAttackReadyState);
    }

    private void EnterAttackReadyState()
    {
        EnterLungeState(LungeStates.AttackReady, 0);
    }

    private void RefreshLungeAbility()
    {
        if (DeflectState is not DeflectStates.Deflecting) SharkBlob.SetBlobState(BlobStates.Normal);
        SharkBlob.SetIsMovementLocked(false);
        EnterAttackReadyState();
    }

    private void OnBlobCollidedWhileProtected()
    {
        if (LungeState is LungeStates.Lunging) lungeHitEnemy = true;
        else if (LungeState is LungeStates.LungeRecovery or LungeStates.OnCooldown) RefreshLungeAbility();

        if (DeflectState is DeflectStates.OnCooldown) DeflectState = DeflectStates.DeflectReady;
    }
#endregion


#region Defend state management methods
    private void ManageDefendState(double delta)
    {
        switch (DeflectState)
        {
            case DeflectStates.DeflectReady:
                bool IsAttackActive = LungeState is LungeStates.Lunging or LungeStates.LungeRecovery;
                bool IsValidDefense = SharkBlob.IsDefendPressed && SharkBlob.BlobState != BlobStates.Suppressed && !IsAttackActive;
                if (IsValidDefense) EnterDeflectingState();
                break;

            case DeflectStates.Deflecting:
                CurrentDeflectStateTimer -= delta;
                bool IsDoneDeflecting = CurrentDeflectStateTimer <= 0;
                if (IsDoneDeflecting) EnterDeflectCooldown();
                break;

            case DeflectStates.OnCooldown:
                CurrentDeflectStateTimer -= delta;
                bool IsDeflectReady = CurrentDeflectStateTimer <= 0;
                if (IsDeflectReady) DeflectState = DeflectStates.DeflectReady;
                break;
        }
    }

    private void EnterDeflectingState()
    {
        SharkBlob.SetBlobState(BlobStates.Protected);
        CurrentDeflectStateTimer = GameConfig.Shark.DeflectWindowSeconds;
        DeflectState = DeflectStates.Deflecting;
    }

    private void EnterDeflectCooldown()
    {
        SharkBlob.SetBlobState(BlobStates.Normal);
        CurrentDeflectStateTimer = GameConfig.Shark.DefendCooldownSeconds;
        DeflectState = DeflectStates.OnCooldown;
    }

#endregion

#region blob state management methods

    private void ManageAfflictingState()
    {
        SharkBlob.SetAfflictingState(AfflictingState);
    }

#endregion

}
