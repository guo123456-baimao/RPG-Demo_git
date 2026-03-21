using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMoveState : SlimeGroundedState
{
    public SlimeMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime enemy) : base(_enemyBase, _stateMachine, _animBoolName, enemy)
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
        if (Vector2.Distance(enemy.transform.position, player.position) < enemy.battleDistance)
        {
            stateMachine.ChangeState(enemy.battleState);
            return;
        }

        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);
    }
}
