using Blobeatblob.Enums.GameStates;
using BlobEatBlob.HelperScripts;
using BlobEatBlob.Scripts;
using Godot;
using System;

namespace BlobEatBlob.GameScripts;

public class LungeAttackModeStateAndTimer
(LungeStates currentLungeState, LungeStates nextLungeState = default, double startTime = default, double endTime = 0, bool increasingTimer = false)
{
    public LungeStates CurrentLungeState { get; set; } = currentLungeState;

    public LungeStates NextLungeState { get; set; } = nextLungeState;

    public double StartTime { get; set; } = startTime;

    public double EndTime { get; set; } = endTime;

    public bool IncreasingTimer {get; set; } = increasingTimer;

    private double CurrentTime = startTime;

    public bool ManageTimer(double delta, bool abort = false)
    {

        if (abort || (IncreasingTimer ? CurrentTime >= EndTime : CurrentTime <= EndTime))
        {
            EndTime = CurrentTime;
            return true;
        }

        CurrentTime = IncreasingTimer
        ? CurrentTime + delta
        : CurrentTime - delta;
 
        return false;
    }
}

public partial class Shark : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private double attackCooldownSecondsLeft = 0;

    private double defendCooldownSecondsLeft = 0;

    private double chargeTimeSeconds = 0;

    private double lungeSecondsLeft = 0;

    private double lungeRecoverySecondsLeft = 0;

    private Blob SharkBlob => GetParent<Blob>();

    LungeAttackModeStateAndTimer LungeStateAndTimer = new(LungeStates.AttackReady);

    private bool IsValidAttack =>
    SharkBlob.BlobState != BlobStates.Suppressed &&
    SharkBlob.IsAttackPressed &&
    LungeStateAndTimer.CurrentLungeState == LungeStates.AttackReady;

    private bool Attacking => LungeStateAndTimer.CurrentLungeState != LungeStates.AttackReady && LungeStateAndTimer.CurrentLungeState != LungeStates.OnCooldown;

    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        LungeStateAndTimer = new(LungeStates.AttackReady);
    }

    public override void _PhysicsProcess(double delta)
    {

        if (IsValidAttack)
        {
            LungeStateAndTimer =
                new(LungeStates.LungeCharging, LungeStates.Lunging, 0, GameConfig.Shark.MaxChargeSeconds, true);
        }
        else if (Attacking)
        {
            ManageAttackMode(delta);
        }
    }

    private void ManageAttackMode(double delta)
    {
        bool IsStateTimerDone = LungeStateAndTimer.ManageTimer(delta, !SharkBlob.IsAttackPressed);
        if (IsStateTimerDone)
        {
            double lungingTimer = LungeStateAndTimer.EndTime * GameConfig.Shark.lungeSecondsPerChargeSecond;
            LungeStateAndTimer = new(LungeStateAndTimer.NextLungeState, startTime: lungingTimer);
        } 

        switch (LungeStateAndTimer.CurrentLungeState)
        {
            case LungeStates.Lunging: StartLunge(); break;
            case LungeStates.LungeRecovery: RecoverFromLunge(delta); break;
            case LungeStates.OnCooldown: CooldownAttack(delta); break;
        }
    }

    private void StartLunge()
    {
        lungeSecondsLeft = Math.Min(chargeTimeSeconds, GameConfig.Shark.MaxChargeSeconds) * GameConfig.Shark.lungeSecondsPerChargeSecond;
        chargeTimeSeconds = 0;

        SharkBlob.Velocity = SharkBlob.AimDirection * (GameConfig.Blob.Speed * GameConfig.Shark.LungeSpeedMultiplier);
        SharkBlob.SetBlobState(BlobStates.Protected);

        LungeStateAndTimer =
                new(LungeStates.Lunging, LungeStates.LungeRecovery, LungeStateAndTimer.StartTime, );
    }

    private void Lunging(double delta)
    {
        lungeSecondsLeft -= delta;
        if (lungeSecondsLeft <= 0) StartLungeRecovery();
    }

    private void StartLungeRecovery()
    {
        lungeSecondsLeft = 0;
        lungeRecoverySecondsLeft = GameConfig.Shark.LungeRecoverySeconds;

        SharkBlob.SetBlobState(BlobStates.Suppressed);

        LungeState = LungeStates.LungeRecovery;
    }

    private void RecoverFromLunge(double delta)
    {
        lungeRecoverySecondsLeft -= delta;
        if (lungeRecoverySecondsLeft <= 0) StartAttackCooldown();
    }

    private void StartAttackCooldown()
    {
        lungeRecoverySecondsLeft = 0;
        attackCooldownSecondsLeft = GameConfig.Shark.AttackCooldownSeconds;

        SharkBlob.SetBlobState(BlobStates.Normal);

        LungeState = LungeStates.OnCooldown;
    }

    private void CooldownAttack(double delta)
    {
        attackCooldownSecondsLeft -= delta;
        if (attackCooldownSecondsLeft <= 0) StartAttackReady();
    }

    private void StartAttackReady()
    {
        attackCooldownSecondsLeft = 0;
        LungeState = LungeStates.AttackReady;
    }
}
