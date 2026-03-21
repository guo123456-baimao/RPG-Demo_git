using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private float lastTImeAttack;
    private float comboWindow = 2;

    public PlayerPrimaryAttackState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


        xinput = 0;           //fix bug
        if (comboCounter > 2 || Time.time >lastTImeAttack+comboWindow)
        {
            comboCounter = 0;
        }
        player.anim.SetInteger("ComboCounter", comboCounter);

        #region  chose attack direction
        float attackDic = player.facingDir;
        if (xinput!=0)
        {
            attackDic = xinput;
        }
        #endregion

        stateTimer = 0.1f;
        player.SetVelocity(player.attackMovement[comboCounter].x* attackDic, player.attackMovement[comboCounter].y);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.15f);
        comboCounter++;
        lastTImeAttack = Time.time;
        
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer<0)
        {
            player.SetZeroVelocity();
        }
        if (triggerCalled)
        {
            stateMachine.changeState(player.idleState);
        }
    }
}
