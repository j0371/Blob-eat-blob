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

    private LungeStates LungeState => true switch
    {
        _ when chargeTimeSeconds > 0 && chargeTimeSeconds < GameConfig.Shark.MaxChargeSeconds && SharkBlob.IsAttackPressed && !(attackCooldownSecondsLeft > 0) => LungeStates.Charging,
        _ when chargeTimeSeconds >= GameConfig.Shark.MaxChargeSeconds || (chargeTimeSeconds > 0 && !SharkBlob.IsAttackPressed) => LungeStates.LungePrimed,
        _ when lungeSecondsLeft > 0 => LungeStates.Lunging,
        _ when lungeRecoverySecondsLeft > 0 => LungeStates.LungeRecovery,
        _ when attackCooldownSecondsLeft > 0 => LungeStates.OnCooldown,
        _ when SharkBlob.IsAttackPressed => LungeStates.ChargePrimed,
        _ => LungeStates.AttackReady
    };

    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _PhysicsProcess(double delta)
    {

        switch (LungeState)
        {
            case LungeStates.AttackReady: WaitForAttack(); break;
            case LungeStates.ChargePrimed: StartCharge(delta); break;
            case LungeStates.Charging: Charge(delta); break;
            case LungeStates.LungePrimed: StartLunge(); break;
            case LungeStates.Lunging: ContinueLunge(delta); break;
            case LungeStates.LungeRecovery: RecoverFromLunge(delta); break;
            case LungeStates.OnCooldown: TickAttackCooldown(delta); break;
        }

        if (LungeState == LungeStates.LungeReady)
        {
            Blob sharkBlob = GetParent<Blob>();

            if (sharkBlob is Player)
            {
                Vector2 mousePosition = GetGlobalMousePosition();

            }

            attackCooldownSecondsLeft = GameConfig.Shark.AttackCooldownSeconds;
            chargeTimeSeconds = 0;
        }
        else if (Input.IsActionPressed(GameConfig.InputActions.Attack))
        {
            chargeTimeSeconds += delta;
        }



    }

    private void Attack()
    {
        
    }

    private void Defend()
    {

    }

    private void BuildAttackCharge(double delta)
    {
        
    }
}
