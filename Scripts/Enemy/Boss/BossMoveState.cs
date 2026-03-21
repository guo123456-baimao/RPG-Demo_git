using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoveState : BossGroundedState
{
    public BossMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _boss) : base(_enemyBase, _stateMachine, _animBoolName, _boss)
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

        // 优先判断：只要距离足够近，直接切Battle，跳过Idle
        if (Vector2.Distance(boss.transform.position, player.position) < boss.battleDistance)
        {
            stateMachine.ChangeState(boss.battleState);
            return;
        }

        if (boss.IsWallDetected() || !boss.IsGroundDetected())
        {
            boss.Flip();
            // 停止水平移动，切回 Idle 并立即返回，避免继续执行后续设置速度逻辑导致抖动/切换
            boss.SetVelocity(0f, rb.velocity.y);
            stateMachine.ChangeState(boss.idleState);
            return;
        }
        boss.SetVelocity(boss.moveSpeed * boss.facingDir, rb.velocity.y);
    }
}
