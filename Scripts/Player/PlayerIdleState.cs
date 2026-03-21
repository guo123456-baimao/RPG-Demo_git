using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState :PlayerGroundedState
{
    public PlayerIdleState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (xinput!=0&&!player.isBusy)
        {
            stateMachine.changeState(player.moveState);

        }
    }
}
