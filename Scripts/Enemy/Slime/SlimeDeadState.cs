using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeadState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.cd.enabled = false;
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        stateTimer = .1f;
        enemy.CloseCounterAttackWindow();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;

        }
    }
}
