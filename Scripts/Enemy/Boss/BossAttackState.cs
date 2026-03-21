using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : EnemyState
{
    private Enemy_Boss boss;
    public BossAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss boss ): base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        boss.lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();
        boss.SetZeroVelocity();
        if (triggerCalled)
        {
            stateMachine.ChangeState(boss.battleState);
        }
    }
}
