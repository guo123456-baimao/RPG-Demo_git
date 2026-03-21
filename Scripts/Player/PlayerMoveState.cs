using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState :PlayerGroundedState
{
    public PlayerMoveState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(14, null);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(14);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xinput*player.moveSpeed, rb.velocity.y);

        if (xinput==0||player.IsWallDetected())
        {
            stateMachine.changeState(player.idleState);
        }
    }
}
