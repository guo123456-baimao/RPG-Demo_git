using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration=.2f;

    public bool isBusy { get; private set; } 


    [Header("Move info")]
    public float moveSpeed = 8f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    [Space]
    public UI ui;
    public GameObject sword { get; private set; }

    public float dashDir { get; private set; }

    public PlayerFX fx {  get; private set; }
    public SkillManager skill { get; private set; }
  

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState walljump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }  

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(stateMachine,this,"Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Move");
        jumpState = new PlayerJumpState(stateMachine, this, "Jump");
        airState  = new PlayerAirState(stateMachine, this, "Jump");
        dashState  = new PlayerDashState(stateMachine, this, "Dash");
        wallSlide = new PlayerWallSlideState(stateMachine, this, "WallSlide");
        walljump = new PlayerWallJumpState(stateMachine, this, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(stateMachine, this, "Attack");
        counterAttack = new PlayerCounterAttackState(stateMachine, this, "CounterAttack");

        aimSword = new PlayerAimSwordState(stateMachine, this, "AimSword");
        catchSword = new PlayerCatchSwordState(stateMachine, this, "CatchSword");

        blackHole = new PlayerBlackholeState(stateMachine, this, "Jump");

        deadState = new PlayerDeadState(stateMachine, this, "Die");
    }

    protected override void Start()
    {
        base.Start();

        fx=GetComponent<PlayerFX>();

        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

   
    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;


        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Inventory.instance.UseFlask();
        }
    }

    public override void SlowEntity(float _slowPercentage, float _slowDuration)
    {
        moveSpeed=moveSpeed*(1- _slowPercentage);
        jumpForce=jumpForce*(1- _slowPercentage);
        dashSpeed=dashSpeed*(1- _slowPercentage);   
        anim.speed=anim.speed*(1- _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;

    }

    public void AssignNewWord(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.changeState(catchSword);
        Destroy(sword);
    }


 

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
      
        isBusy = false;
    }

    
    public void CheckForDashInput()
    {
        
        if (IsWallDetected())
            return;
        
        if(skill.dash.dashUnlocked==false)
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift)&&SkillManager.instance.dash.CanUseSkill())
        {
            
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir==0)
            {
                dashDir = facingDir;
            }
            stateMachine.changeState(dashState);
        }
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();


    public override void Die()
    {
        base.Die();
        // 掉落与保存由 PlayerStats.Die() 统一处理，避免重复
        stateMachine.changeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }

    public  void SetChanceForPlace(ChanceForPlace targetPlace)
    {
        if(rb!=null)
            rb.position = targetPlace.transform.position;
    }
}
