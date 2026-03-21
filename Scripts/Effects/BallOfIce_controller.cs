using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOfIce_controller : MonoBehaviour
{
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public float canToDistance;

    private Player player;
    private Vector2 moveDic;
    private Vector2 startPos;
    private Vector2 currentPos;
    [SerializeField] private float moveSpeed;
    private Animator anim => GetComponent<Animator>();
    // 标记是否为有效发射（避免无发射时触发回收）
    private bool isLaunched = false;

    private void OnEnable()
    {
        startPos = transform.position;
        isLaunched = false;
        player = PlayerManager.instance.player;
    }

    // 发射时初始化朝向和参数
    public void Init(Vector2 launchDir, float maxDistance, int damageVal)
    {
        moveDic = launchDir.normalized; 
        canToDistance = maxDistance;
        damage = damageVal;
        startPos = transform.position; 
        isLaunched = true; 
    }

    private void Update()
    {
        if (!isLaunched) return; 

        currentPos = transform.position;
        transform.Translate(moveDic * moveSpeed * Time.deltaTime);
        float distance = Vector2.Distance(currentPos, startPos);

        if (distance > canToDistance && gameObject.activeSelf)
        {
            RecycleSelf();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            anim.SetTrigger("bust");
            player = collision.GetComponent<Player>();
            isLaunched = false; 
        }
    }

    public void PustTrigger()
    {
        player.stats.TakeDamage(damage);
    }

    public void AnimationFinishTrigger()
    {
        RecycleSelf();
    }

    private void RecycleSelf()
    {
        isLaunched = false;
        ObjectPoolManager.instance.RecycleObj(ObjectPoolManager.instance.slime_BallOfIce_Prefab, gameObject);
    }
}