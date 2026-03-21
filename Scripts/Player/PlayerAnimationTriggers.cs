using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    Player Player => GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        Player.AnimationTrigger();
    }
    private void AttackTriggger()
    {
        AudioManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(Player.attackCheck.position, Player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                    Player.stats.DoDamage(_target);


                ItemData_Equipment weapomData = Inventory.instance.GetEquipment(EquipmentType.Weapon);            //通过武器类型获取装备
                if (weapomData != null)
                {
                    weapomData.Effect(_target.transform);                                                        //执行特效
                }
            }
        }
    }
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }

}
