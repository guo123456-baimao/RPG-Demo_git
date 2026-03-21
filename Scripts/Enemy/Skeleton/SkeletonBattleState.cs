using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton enemy;
    private int moveDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.battleTime;
        player = PlayerManager.instance.player.transform;
        enemy.moveSpeed *= 2f;
        if(player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsPlayerDetected())
        {
            if (enemy.IsPlayerDetected().distance<enemy.attackdistance)
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                    
                }

            }
        }
        else
        {
            if (stateTimer<0||Vector2.Distance(player.transform.position,enemy.transform.position)>enemy.battleDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        if (player.position.x>enemy.transform.position.x)
        {
            moveDir = 1;
        }else if (player.position.x<enemy.transform.position.x)
        {
            moveDir = -1;
        }

        if (Mathf.Abs(player.position.x - enemy.transform.position.x) < 1.3f)
            return;
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);

    }
    public override void Exit()
    {
        base.Exit();
        enemy.moveSpeed /= 2f;
    }
    private bool CanAttack()
    {
        if (Time.time>enemy.lastTimeAttack+enemy.attackCooldown)
        {
            enemy.attackCooldown=Random.Range(enemy.minAttackCooldown,enemy.maxAttackCooldown);

            enemy.lastTimeAttack = Time.time;
            return true;
        }
        return false;
    }

}
