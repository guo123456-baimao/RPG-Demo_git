using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1f;
        player.SetVelocity(5 * -player.facingDir,player.jumpForce);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer<0)
        {
            stateMachine.changeState(player.airState);
        }
        if (player.IsGroundDetected())
        {
            stateMachine.changeState(player.idleState);
        }
    }
}
