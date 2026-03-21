using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleState : EnemyState
{
    private Transform player;
    private Enemy_Boss boss;
    private int moveDir;
    public BossBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boss.battleTime;
        boss.moveSpeed *= 3f;
        player = PlayerManager.instance.player.transform;
        if(player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(boss.moveState);
    }

    public override void Update()
    {
        base.Update();
        if (boss.IsPlayerDetected())
        {
            if (boss.IsPlayerDetected().distance < boss.attackdistance)
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(boss.attackState);                 
                }

            }
        }
        else
        {
            if (stateTimer < 0 || (Vector2.Distance(player.transform.position, boss.transform.position) > boss.battleDistance &&!player.GetComponent<PlayerStats>().isDead))
            {
                stateMachine.ChangeState(boss.idleState);
            }
        }

        if (player.position.x > boss.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < boss.transform.position.x)
        {
            moveDir = -1;
        }

        if (Mathf.Abs(player.position.x - boss.transform.position.x) < 4f)
            return;
        boss.SetVelocity(boss.moveSpeed * moveDir, rb.velocity.y);

    }
    public override void Exit()
    {
        base.Exit();
        boss.moveSpeed /= 3f;
    }
    private bool CanAttack()
    {
        if (Time.time > boss.lastTimeAttack + boss.attackCooldown)
        {
            boss.attackCooldown = Random.Range(boss.minAttackCooldown, boss.maxAttackCooldown);

            boss.lastTimeAttack = Time.time;
            return true;
        }
        return false;
    }


}
