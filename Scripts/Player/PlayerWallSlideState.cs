using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
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

        if (player.IsWallDetected() == false)
            stateMachine.changeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.changeState(player.walljump);
            return;
        }

        if (xinput!=0&&player.facingDir!=xinput)
        {
            stateMachine.changeState(player.idleState);
        }

        if (yinput<0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
        rb.velocity = new Vector2(0, rb.velocity.y * 0.7f);
        }

        if (player.IsGroundDetected())
        {
            stateMachine.changeState(player.idleState);
        }
    }
}
