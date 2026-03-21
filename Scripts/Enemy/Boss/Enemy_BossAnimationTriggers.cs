using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BossAnimationTriggers : MonoBehaviour
{
    private Enemy_Boss boss => GetComponentInParent<Enemy_Boss>();
    private void AnimationTrigger()
    {
        boss.AnimationFinishTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(boss.attackCheck.position, boss.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats _target = hit.GetComponent<PlayerStats>();
                boss.stats.DoDamage(_target);
                boss.comboAttackCount++;
            }
        }
    }

    private void OpenCountWindow() => boss.OpenCounterAttackWindow();
    private void CloseCountWindow() => boss.CloseCounterAttackWindow();

    private void ChangeBossStateToIdle()=> boss.stateMachine.ChangeState(boss.idleState);


    private void CheckCanUseSkillFirst()
    {
        if (boss.comboAttackCount == 4)                              //攻击满足3次后施放1技能
        {
            AudioManager.instance.PlaySFX(19, boss.transform);
            boss.stateMachine.ChangeState(boss.skillFirstState);
            boss.comboAttackCount = 0;
        }
    }

}
