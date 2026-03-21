using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGroundedState : EnemyState
{
    protected Enemy_Boss boss;
    protected Transform player;
    public BossGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss _boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = _boss;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void Update()
    {
        base.Update();
        if (boss.IsPlayerDetected()||Vector2.Distance(boss.transform.position,player.position)<boss.battleDistance)
        {
            
            stateMachine.ChangeState(boss.battleState);
               
        }
    }
}
