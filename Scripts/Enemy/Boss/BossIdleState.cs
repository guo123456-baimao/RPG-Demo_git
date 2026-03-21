using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : BossGroundedState
{
    public BossIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _boss) : base(_enemyBase, _stateMachine, _animBoolName, _boss)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boss.idleTime;
    }

    public override void Exit() 
    { 
        base.Exit(); 
        AudioManager.instance.PlaySFX(24,boss.transform);
    }

    public override void Update()
    {
        base.Update();
        if (Vector2.Distance(boss.transform.position, player.position) < boss.battleDistance)
        {
            stateMachine.ChangeState(boss.battleState);
            return; // ÖÕÖ¹ºóÐøIdle¡úMoveµÄÂß¼­
        }
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.moveState);
        }
    }



}
