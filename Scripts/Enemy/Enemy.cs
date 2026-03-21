using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity                              //µ–»À-ª˘¿‡
{
    [SerializeField ]protected LayerMask whatIsPlayer;
    public EnemyStateMachine stateMachine { get; private set; }

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;
    
    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackdistance;
    public float attackCooldown;
    public float minAttackCooldown;
    public float maxAttackCooldown;

    [Header("Battle info")]
    public float battleTime;
    public float battleDistance;

    private int playerDetectTimerCount = 0;
    private int MindetectTime=2;

    [HideInInspector]public float lastTimeAttack;

    public string lastAnimBoolName { get;private set; }

    public EntityFX fx { get; private set; }



    protected override void Awake()
    {
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;

    }


    protected override void Start()
    {
        base.Start();

        fx = GetComponentInChildren<EntityFX>();
    }




    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        
    }

    public override void SlowEntity(float _slowPercentage, float _slowDuration)
    {
        moveSpeed= defaultMoveSpeed * (1 - _slowPercentage);
        anim.speed =anim.speed*( 1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }


    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    public virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion
    
    
    public virtual bool CheckBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }


    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.CircleCast(attackCheck.position, attackCheckRadius,Vector2.right*facingDir,0,whatIsPlayer);
        // Physics2D.Raycast(attackCheck.position, Vector2.right * facingDir, 10, whatIsPlayer);

    

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackdistance * facingDir, transform.position.y));
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }



}
