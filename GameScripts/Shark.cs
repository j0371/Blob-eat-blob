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

    private double CurrentLungeStateTimer;

    private Blob SharkBlob => GetParent<Blob>();


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _PhysicsProcess(double delta)
    {
        ManageAttackState(delta);
    }

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

    private void Defend()
    {

    }
}
