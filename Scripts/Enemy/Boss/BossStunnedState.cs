using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStunnedState : EnemyState
{
    Enemy_Boss boss;
    public BossStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        boss.fx.InvokeRepeating("RedColorBlink", 0, .1f);
        stateTimer = boss.stunDuration;
        rb.velocity = new Vector2(-boss.facingDir * boss.stunDirection.x, boss.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        boss.fx.Invoke("CancelColorChange", 0);

    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }
}
