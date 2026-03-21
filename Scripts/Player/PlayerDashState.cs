using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{

    public PlayerDashState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.dash.CloneOnDash();
        stateTimer = player.dashDuration;

        player.stats.MakeInvicible(true);

    }

    public override void Exit()
    {
        base.Exit();
        player.skill.dash.CloneOnArrival();
        rb.velocity = new Vector2(0, rb.velocity.y);

        player.stats.MakeInvicible(false);
    }

    public override void Update()
    {
        base.Update();

        rb.velocity = new Vector2(player.dashSpeed * player.dashDir, 0);

        if (stateTimer<0)
        {
            stateMachine.changeState(player.idleState);
        }
        if (!player.IsGroundDetected()&&player.IsWallDetected())
        {
            stateMachine.changeState(player.wallSlide);
        }

        player.fx.CreateAfterImage();



    }
}
    

