using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour                    //实体-基类
{
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    #endregion
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected float knockbackDuration;
    protected bool isknocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int knockbackDir {  get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action OnFlipped;


    protected virtual void Awake()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    public virtual void SlowEntity(float _slowPercentage,float _slowDuration)
    {
        
    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }


    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }


    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;

    }


    public void SetupKnockbackPower(Vector2 _knockbackPower)
    {
        knockbackPower = _knockbackPower;
    }



    protected virtual IEnumerator HitKnockback()
    {
        isknocked = true;
        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
        yield return new WaitForSeconds(knockbackDuration);
        isknocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }




    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);


    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        
        if (OnFlipped!=null)
        {
            OnFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }

    }
    #endregion

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isknocked)
        {
            return;
        }

        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isknocked)
        {
            return;
        }
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    
    public virtual void Die()
    {

    }



}
