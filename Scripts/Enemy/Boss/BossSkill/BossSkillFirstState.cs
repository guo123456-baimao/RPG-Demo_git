using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillFirstState : EnemyState
{
    private Enemy_Boss boss;
    public BossSkillFirstState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        boss.StartCoroutine(boss.InitLineOfFire(boss.LineCount));
    }

    public override void Exit()
    {
        base.Exit();
        boss.StartCoroutine(boss.recycleLineOfFire());
    }

    public override void Update()
    {
        base.Update();
    }

    


}