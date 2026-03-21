using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (player.IsWallDetected())
        {
            stateMachine.changeState(player.wallSlide);
        }
        if (player.IsGroundDetected())
        {
            stateMachine.changeState(player.idleState);
        }
        if (xinput!=0)
        {
            player.SetVelocity(player.moveSpeed*0.8f*xinput,rb.velocity.y);
        }
    }
}
