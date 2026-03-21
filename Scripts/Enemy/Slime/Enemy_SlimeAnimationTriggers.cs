using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SlimeAnimationTriggers : MonoBehaviour
{
    private Enemy_Slime enemy => GetComponentInParent<Enemy_Slime>();
    [SerializeField] private float cd = 3f;
    private float duringTime;

    private void Update()
    {
        duringTime -= Time.deltaTime;
    }

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null && duringTime < 0)
            {
                GameObject iceBall = ObjectPoolManager.instance.GetObj(ObjectPoolManager.instance.slime_BallOfIce_Prefab);
                iceBall.transform.position = enemy.transform.position;
                Vector2 launchDir = hit.transform.position - enemy.transform.position;
                iceBall.GetComponent<BallOfIce_controller>().Init(
                    launchDir,
                    enemy.attackdistance,
                    enemy.stats.damaged.GetValue()
                );
                duringTime = cd;
            }
        }
    }

    private void OpenCountWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCountWindow() => enemy.CloseCounterAttackWindow();
}