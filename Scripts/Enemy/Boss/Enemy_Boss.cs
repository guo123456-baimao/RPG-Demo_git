using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    public BossIdleState idleState { get; private set; }
    public BossMoveState moveState { get; private set; }
    public BossBattleState battleState { get; private set; }
    public BossAttackState attackState { get; private set; }
    public BossStunnedState stunnedState { get; private set; }
    public BossDeadState deadState { get; private set; }
    public BossSkillFirstState skillFirstState{ get; private set; }

    [Header("Boss技能")]
    public int comboAttackCount=0;
    public int LineCount = 3;
    [SerializeField] private GameObject lineOfFire;
    private Stack<GameObject> lineOfFirePool;
    private GameObject line;

    [Space]
    [SerializeField] private GameObject youWin;
    public UI ui;

    protected override void Awake()
    {
        base.Awake();
        idleState = new BossIdleState(this, stateMachine, "Idle", this);
        moveState = new BossMoveState(this, stateMachine, "Move", this);
        battleState = new BossBattleState(this, stateMachine, "Move", this);
        attackState = new BossAttackState(this, stateMachine, "Attack", this);
        stunnedState = new BossStunnedState(this, stateMachine, "Stunned", this);
        deadState = new BossDeadState(this, stateMachine, "Death", this);

        skillFirstState = new BossSkillFirstState(this, stateMachine, "SkillFirst", this);

        lineOfFirePool = new Stack<GameObject>();   

        if (ObjectPoolManager.instance!=null)
        {
            ObjectPoolManager.instance.PreWarmPool(lineOfFire, 6);
        }
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }


    public override bool CheckBeStunned()
    {
        if (base.CheckBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public IEnumerator InitLineOfFire(int count)
    {
        if (ObjectPoolManager.instance!=null)
        {
            for (int i = 0; i < count; i++)
            {
                line= ObjectPoolManager.instance.GetObj(lineOfFire);                    //从池中获取一个LineOfFire对象，并设为活跃
                line.transform.position = PlayerManager.instance.player.transform.position;
                line.transform.rotation=Quaternion.Euler(0, 0, -90);
                lineOfFirePool.Push(line);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public IEnumerator recycleLineOfFire()
    {
        yield return new WaitForSeconds(3f);
        if (ObjectPoolManager.instance!=null)
        {
            for (int i = 0; i < LineCount; i++)
            {
                ObjectPoolManager.instance.RecycleObj(lineOfFire, lineOfFirePool.Pop());            //将LineOfFire对象回收至池中，并设为不活跃
            }
            
        }
    }

}
