using System.Collections;
using UnityEngine;



public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damaged,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage,

}



public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;                     //强度
    public Stat agility;                      //敏捷
    public Stat intelligence;                 //智力
    public Stat vitality;                     //体力

    [Header("Offensive stats")]
    public Stat damaged;                      //伤害
    public Stat critChance;                   //暴击率
    public Stat critPower;                    //暴击强度


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;                         //护甲
    public Stat evasion;                       //闪避
    public Stat magicResisence;               //魔法抗性

    [Header("Magic stats")]
    public Stat fireDamage;                 //火焰伤害  
    public Stat iceDamage;                  //冰霜伤害
    public Stat lightingDamage;            //闪电伤害

    public bool isIgnited;               //是否点燃
    public bool isChilled;                //是否冰冻
    public bool isShocked;               //是否感电

    [SerializeField] private float ailmentDuration=4; //状态持续时间
    private float ignitedTimer;        //点燃计时器
    private float chilledTimer;       //冰冻计时器
    private float shockedTimer;      //感电计时器


    private float igniteDamageCooldown=.3f; //点燃伤害间隔
    private float igniteDamageTimer;      //点燃伤害计时器
    private int igniteDamage;          //点燃伤害
    private int shockDamge;          //感电伤害

    [SerializeField] private GameObject shockStrikePrefab;

    public int currentHealth;

    public System.Action OnHealthChanged;

    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth =GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        if (shockedTimer < 0)
        {
            isShocked = false;
        }

        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }
    
    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCoroutine(_duration));
    }

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable=false;
    }

   
    public virtual void IncreaseStatsBy(int _modifier,float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier,float _duration,Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }


    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool criticalStrike= false;


        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damaged.GetValue() + strength.GetValue();

        if (CanCrit())
        {
           totalDamage= CalculateCritDamage(totalDamage);
            criticalStrike= true;   
        }

        fx.CreateHitFX(_targetStats.transform,criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);                   //普通伤害

        DoMagicDamage(_targetStats);                          //魔法伤害
    }


    #region Magic Damage and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
        

    }

    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 0.5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }
        if (canApplyShock)
        {
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
      
    }

    public void ApplyAilments(bool _ignite,bool _chill,bool _shock)
    {
        bool canApplyIgnite =!isIgnited && !isChilled && !isShocked;                    //有异常状态时无法再被应用异常状态
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;                    //有异常状态时无法再被应用异常状态
        bool canApplyShock = !isIgnited && !isChilled;                                  //有点燃或冰冻状态时无法被应用感电状态


        if (_ignite&&canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentDuration;
            fx.IgniteFxFor(ailmentDuration);
        }
        if (_chill&&canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentDuration;

            float slowPercentage = 0.2f;
            GetComponent<Entity>().SlowEntity(slowPercentage, ailmentDuration);
            fx.ChillFxFor(ailmentDuration);
        }
        if (_shock&&canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)                            //如果是玩家二次触发则直接返回
                {
                    return;
                }

                HitNearestTargetWithShockStricke();                           //如果是敌人二次触发则释放闪电链
            }

        }
        

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
        {
            return;
        }
        isShocked = _shock;
        shockedTimer = ailmentDuration;
        fx.ShockFxFor(ailmentDuration);
    }

    private void HitNearestTargetWithShockStricke()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)                           //如果没有找到敌人则返回自己
            {
                closestEnemy = transform;
            }

        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().SetUp(shockDamge, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }

    public void SetupShockStrikeDamage(int _damage)
    {
        shockDamge = _damage;
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0 )
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0&&!isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    #endregion




    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0&&!isDead)
        {
            Die();
        }
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth>GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }
        if (OnHealthChanged!=null)
        {
            OnHealthChanged();
        }

    }


    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;

        if(_damage > 0) 
            fx.CreatePopUpText(_damage.ToString());


        if(OnHealthChanged!=null)
        {
            OnHealthChanged();
        }
    }
    protected virtual void Die()
    {
        isDead=true;
    }

    public virtual void OnEvasion()
    {

    }



    #region Stat calculations



    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
            
        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    protected bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }
        return false;
    }
    protected int CalculateCritDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResisence.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }
    #endregion

    public void KillEntity()
    {
        if(!isDead)
            Die();
    }

    public void MakeInvicible(bool _invicible)
    {
        isInvincible = _invicible;
    }

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damaged) return damaged;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResisence;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }




}
