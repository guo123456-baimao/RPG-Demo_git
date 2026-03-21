using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;
    private SwordType _defaultSwordType;
    private float _defaultSwordGravity;
    private GameObject InGame_UI;

    [Header("Bounce info")]
    [SerializeField] private UI_skillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Peirce info")]
    [SerializeField] private UI_skillTreeSlot peirceUnlockButton;
    [SerializeField] private int peirceAmount;
    [SerializeField] private float peirceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_skillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private UI_skillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_skillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_skillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked { get; private set; }

    private Vector2 finalDir;

    [Header("Aim info")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float SpaceBeetwenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        InGame_UI= Resources.FindObjectsOfTypeAll<UI_inGame>()[0].gameObject;
        _defaultSwordType = swordType;
        _defaultSwordGravity = swordGravity; // 保存 Inspector 中的默认重力

        // 初始同步并订阅
        if (swordUnlockButton != null)
        {
            swordUnlocked = swordUnlockButton.unlocked;
            swordUnlockButton.OnStateChanged += OnSwordUnlockChanged;
        }

        if (bounceUnlockButton != null)
        {
            if (bounceUnlockButton.unlocked) swordType = SwordType.Bounce;
            bounceUnlockButton.OnStateChanged += OnBounceChanged;
        }

        if (peirceUnlockButton != null)
        {
            if (peirceUnlockButton.unlocked) swordType = SwordType.Pierce;
            peirceUnlockButton.OnStateChanged += OnPeirceChanged;
        }

        if (spinUnlockButton != null)
        {
            if (spinUnlockButton.unlocked) swordType = SwordType.Spin;
            spinUnlockButton.OnStateChanged += OnSpinChanged;
        }

        if (timeStopUnlockButton != null)
        {
            timeStopUnlocked = timeStopUnlockButton.unlocked;
            timeStopUnlockButton.OnStateChanged += OnTimeStopChanged;
        }

        if (vulnerableUnlockButton != null)
        {
            vulnerableUnlocked = vulnerableUnlockButton.unlocked;
            vulnerableUnlockButton.OnStateChanged += OnVulnerableChanged;
        }

        // 生成瞄准点
        if(swordUnlocked)
            GenereateDots();

        // 根据当前 swordType 初始化 gravity
        SetupGravity();
    }

    private void OnDestroy()
    {
        if (swordUnlockButton != null) swordUnlockButton.OnStateChanged -= OnSwordUnlockChanged;
        if (bounceUnlockButton != null) bounceUnlockButton.OnStateChanged -= OnBounceChanged;
        if (peirceUnlockButton != null) peirceUnlockButton.OnStateChanged -= OnPeirceChanged;
        if (spinUnlockButton != null) spinUnlockButton.OnStateChanged -= OnSpinChanged;
        if (timeStopUnlockButton != null) timeStopUnlockButton.OnStateChanged -= OnTimeStopChanged;
        if (vulnerableUnlockButton != null) vulnerableUnlockButton.OnStateChanged -= OnVulnerableChanged;
    }

    private void OnSwordUnlockChanged(UI_skillTreeSlot slot, bool newState)
    {
        swordUnlocked = newState;
        if (newState && !bounceUnlockButton.unlocked && !peirceUnlockButton.unlocked && !spinUnlockButton.unlocked)
            swordType = SwordType.Regular;
        else if (!newState)
            swordType = _defaultSwordType;

        SetupGravity();
    }

    private void OnBounceChanged(UI_skillTreeSlot slot, bool newState)
    {
        if (newState) swordType = SwordType.Bounce;
        else if (peirceUnlockButton != null && peirceUnlockButton.unlocked) swordType = SwordType.Pierce;
        else if (spinUnlockButton != null && spinUnlockButton.unlocked) swordType = SwordType.Spin;
        else if (swordUnlockButton != null && swordUnlockButton.unlocked) swordType = SwordType.Regular;
        else swordType = _defaultSwordType;

        SetupGravity();
    }

    private void OnPeirceChanged(UI_skillTreeSlot slot, bool newState)
    {
        if (newState) swordType = SwordType.Pierce;
        else if (bounceUnlockButton != null && bounceUnlockButton.unlocked) swordType = SwordType.Bounce;
        else if (spinUnlockButton != null && spinUnlockButton.unlocked) swordType = SwordType.Spin;
        else if (swordUnlockButton != null && swordUnlockButton.unlocked) swordType = SwordType.Regular;
        else swordType = _defaultSwordType;

        SetupGravity();
    }

    private void OnSpinChanged(UI_skillTreeSlot slot, bool newState)
    {
        if (newState) swordType = SwordType.Spin;
        else if (bounceUnlockButton != null && bounceUnlockButton.unlocked) swordType = SwordType.Bounce;
        else if (peirceUnlockButton != null && peirceUnlockButton.unlocked) swordType = SwordType.Pierce;
        else if (swordUnlockButton != null && swordUnlockButton.unlocked) swordType = SwordType.Regular;
        else swordType = _defaultSwordType;

        SetupGravity();
    }

    private void OnTimeStopChanged(UI_skillTreeSlot slot, bool newState) => timeStopUnlocked = newState;
    private void OnVulnerableChanged(UI_skillTreeSlot slot, bool newState) => vulnerableUnlocked = newState;

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if (swordType == SwordType.Pierce)
        {
            swordGravity = peirceGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
        else
        {
            swordGravity = _defaultSwordGravity;
        }
    }

    protected override void Update()
    {
        // 按下右键时显示瞄准线
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!swordUnlocked||InGame_UI.activeSelf==false) return; // 如果剑技能未解锁或inGame UI 不活跃，直接返回

            if (dots == null || dots.Length == 0) GenereateDots();
            SetupGravity(); // 确保瞄准线使用当前类型的 gravity
            DotsActive(true);
        }

        // 释放右键时记录方向并隐藏瞄准线
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
            DotsActive(false);
        }

        // 持续按住时更新点位置
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (dots == null) return;
            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i] != null)
                    dots[i].transform.position = DotsPosition(i * SpaceBeetwenDots);
            }
        }

    }
    public void CreateSword()
    {
        AudioManager.instance.PlaySFX(27,player.transform);
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetBounce(true, bounceAmount, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(peirceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        }

        SetupGravity(); // 确保实际传入的 gravity 是当前类型的值
        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);
        player.AssignNewWord(newSword);
        DotsActive(false);
    }

    #region  Unlock region

    protected override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPeirceSword();
        UnlockTimeStop();
        UnlockVulnerable();
    }


    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVulnerable()
    {
        if (vulnerableUnlockButton.unlocked)
            vulnerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPeirceSword()
    {
        if (peirceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }



    #endregion


    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;
        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        if (dots == null) return;
        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i] != null) dots[i].SetActive(_isActive);
        }
    }
    private void GenereateDots()
    {
        if (dotPrefab == null || numberOfDots <= 0) return;
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            Vector3 spawnPos = (player != null) ? player.transform.position : Vector3.zero;
            dots[i] = Instantiate(dotPrefab, spawnPos, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    public Vector2 DotsPosition(float t)                               //加速度公式  vt+1/2*a*t^2
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }

    #endregion
}
