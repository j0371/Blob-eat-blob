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
    private double attackCooldownSecondsLeft = 0;

    private double defendCooldownSecondsLeft = 0;

    private double chargeTimeSeconds = 0;

    private double lungeSecondsLeft = 0;

    private double lungeRecoverySecondsLeft = 0;

    private Blob SharkBlob => GetParent<Blob>();

    private double LungeSeconds => Math.Min(chargeTimeSeconds, GameConfig.Shark.MaxChargeSeconds) * GameConfig.Shark.lungeSecondsPerChargeSecond;

    private LungeStates LungeState => true switch
    {
        _ when chargeTimeSeconds > 0 &&
                chargeTimeSeconds < GameConfig.Shark.MaxChargeSeconds &&
                SharkBlob.IsAttackPressed &&
                !(attackCooldownSecondsLeft > 0)
            => LungeStates.Charging,
        _ when chargeTimeSeconds >= GameConfig.Shark.MaxChargeSeconds || (chargeTimeSeconds > 0 && !SharkBlob.IsAttackPressed) => LungeStates.LungePrimed,
        _ when lungeSecondsLeft > 0 => LungeStates.Lunging,
        _ when lungeSecondsLeft == 0 => LungeStates.LungeRecoveryPrimed,
        _ when lungeRecoverySecondsLeft > 0 => LungeStates.LungeRecovery,
        _ when attackCooldownSecondsLeft > 0 => LungeStates.OnCooldown,
        _ when SharkBlob.IsAttackPressed => LungeStates.ChargePrimed,
        _ => LungeStates.AttackReady,
    };

    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _PhysicsProcess(double delta)
    {

        Attack(delta);

    }

    private void Attack(double delta)
    {
        switch (LungeState)
        {
            case LungeStates.AttackReady: break;
            case LungeStates.ChargePrimed: BuildAttackCharge(delta); break;
            case LungeStates.Charging: BuildAttackCharge(delta); break;
            case LungeStates.LungePrimed: StartLunge(); break;
            case LungeStates.Lunging: Lunging(delta); break;
            case LungeStates.LungeRecoveryPrimed: StartLungeRecovery(); break;
            case LungeStates.LungeRecovery: RecoverFromLunge(delta); break;
            case LungeStates.OnCooldown: CooldownAttack(delta); break;
        }
    }

    private void Defend()
    {

    }

    private void BuildAttackCharge(double delta)
    {
        chargeTimeSeconds += delta;
    }

    private void StartLunge()
    {
        SharkBlob.SetBlobState(BlobStates.Protected);
        SharkBlob.Velocity = SharkBlob.AimDirection * (GameConfig.Blob.Speed * GameConfig.Shark.LungeSpeedMultiplier);

        attackCooldownSecondsLeft = GameConfig.Shark.AttackCooldownSeconds;
        lungeSecondsLeft = LungeSeconds;
        chargeTimeSeconds = 0;
    }

    private void Lunging(double delta)
    {
        lungeSecondsLeft = Math.Max(lungeSecondsLeft- delta, 0);
    }

    private void StartLungeRecovery()
    {
        SharkBlob.SetBlobState(BlobStates.Suppressed);
        lungeRecoverySecondsLeft = GameConfig.Shark.LungeRecoverySeconds;
    }

    private void RecoverFromLunge(double delta)
    {
        lungeRecoverySecondsLeft = Math.Max(lungeRecoverySecondsLeft - delta, 0);
    }

    private void CooldownAttack(double delta)
    {
        attackCooldownSecondsLeft = Math.Max(attackCooldownSecondsLeft - delta, 0);
    }
}
