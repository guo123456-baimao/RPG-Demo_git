using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkeletonAnimationTriggers : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();
    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>()!=null)
            {
                PlayerStats _target=hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(_target);
            }
        }
    }

    private void OpenCountWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCountWindow() => enemy.CloseCounterAttackWindow();
}
