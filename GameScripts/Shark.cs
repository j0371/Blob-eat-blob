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

    private double CurrentDeflectStateTimer = GameConfig.Shark.DeflectWindowSeconds;

    private Blob SharkBlob => GetParent<Blob>();

    //0 = attack ready, 1 = attack just used
    public double CooldownRemainingFraction => LungeState switch
    {
        LungeStates.Lunging or LungeStates.LungeRecovery => 1,
        LungeStates.OnCooldown => CurrentLungeStateTimer / GameConfig.Shark.AttackCooldownSeconds,
        _ => 0
    };

    public bool IsChargingLunge => LungeState == LungeStates.LungeCharging;

    //0 = no charge, 1 = fully charged
    public double ChargeFraction => IsChargingLunge ? Math.Min(CurrentLungeStateTimer / GameConfig.Shark.MaxChargeSeconds, 1) : 0;


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
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
                bool IsValidAttack = SharkBlob.IsAttackPressed && SharkBlob.BlobState != BlobStates.Suppressed;
                if (IsValidAttack) EnterLungeChargingState();
                break;

            case LungeStates.LungeCharging:
                CurrentLungeStateTimer += delta;
                bool IsLungeCharged = CurrentLungeStateTimer >= GameConfig.Shark.MaxChargeSeconds || !SharkBlob.IsAttackPressed;
                if (IsLungeCharged) EnterLungingState();
                break;

            case LungeStates.Lunging:
                CurrentLungeStateTimer -= delta;
                bool IsDoneLunging = CurrentLungeStateTimer <= 0;
                if (IsDoneLunging) EnterLungeRecoveryState();
                break;

            case LungeStates.LungeRecovery:
                CurrentLungeStateTimer -= delta;
                bool IsDoneRecovering = CurrentLungeStateTimer <= 0;
                if (IsDoneRecovering) EnterLungeCooldownState();
                break;

            case LungeStates.OnCooldown:
                CurrentLungeStateTimer -= delta;
                bool IsAttackReady = CurrentLungeStateTimer <= 0;
                if (IsAttackReady) EnterAttackReadyState();
                break;
        }
    }

    private void EnterLungeState(LungeStates state, double timer)
    {
        LungeState = state;
        CurrentLungeStateTimer = timer;
    }

    private void EnterLungeChargingState()
    {
        EnterLungeState(LungeStates.LungeCharging, 0);
    }

    private void EnterLungingState()
    {
        SharkBlob.SetBlobState(BlobStates.Protected);
        SharkBlob.Velocity = SharkBlob.AimDirection * (GameConfig.Blob.Speed * GameConfig.Shark.LungeSpeedMultiplier);
        SharkBlob.SetIsMovementLocked(true);

        double LungingTimer = Math.Min(CurrentLungeStateTimer, GameConfig.Shark.MaxChargeSeconds) * GameConfig.Shark.lungeSecondsPerChargeSecond;
        EnterLungeState(LungeStates.Lunging, LungingTimer);
    }

    private void EnterLungeRecoveryState()
    {
        SharkBlob.SetBlobState(BlobStates.Suppressed);
        SharkBlob.Velocity = Vector2.Zero;
        EnterLungeState(LungeStates.LungeRecovery, GameConfig.Shark.LungeRecoverySeconds);
    }

    private void EnterLungeCooldownState()
    {
        SharkBlob.SetBlobState(BlobStates.Normal);
        SharkBlob.SetIsMovementLocked(false);
        EnterLungeState(LungeStates.OnCooldown, GameConfig.Shark.AttackCooldownSeconds);
    }

    private void EnterAttackReadyState()
    {
        EnterLungeState(LungeStates.AttackReady, 0);
    }
#endregion


#region Defend state management methods
    private void ManageDefendState(double delta)
    {
        switch (DeflectState)
        {
            case DeflectStates.DeflectReady:
                bool IsValidDefense = SharkBlob.IsDefendPressed && SharkBlob.BlobState != BlobStates.Suppressed;
                if (IsValidDefense) EnterDeflectingState();
                break;

            case DeflectStates.Deflecting:
                CurrentDeflectStateTimer -= delta;
                bool IsLungeCharged = CurrentLungeStateTimer >= GameConfig.Shark.MaxChargeSeconds || !SharkBlob.IsAttackPressed;
                if (IsLungeCharged) EnterLungingState();
                break;

            case DeflectStates.OnCooldown:
                CurrentLungeStateTimer -= delta;
                bool IsDoneLunging = CurrentLungeStateTimer <= 0;
                if (IsDoneLunging) EnterLungeRecoveryState();
                break;
        }
    }

    private void EnterDeflectingState()
    {
        SharkBlob.SetBlobState(BlobStates.Protected);
        DeflectState = DeflectStates.Deflecting;
    }

    private void EnterDeflectCooldown()
    {
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
