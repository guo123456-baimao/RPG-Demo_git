using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadState : EnemyState
{
    private Enemy_Boss boss;
    public BossDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.boss = boss;
    }
    public override void Enter()
    {
        base.Enter();
        boss.cd.enabled = false;
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        stateTimer = .1f;
        boss.CloseCounterAttackWindow();
        boss.StartCoroutine(YouWin());
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }
    }

    IEnumerator YouWin()
    {
        yield return new WaitForSeconds(4f);
        GameManager.instence.PauseGame(true);
        boss.ui.SwitchYouWin();
    }
}
