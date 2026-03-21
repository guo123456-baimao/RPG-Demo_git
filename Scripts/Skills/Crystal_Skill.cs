using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("crystal mirage")]
    [SerializeField] private UI_skillTreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_skillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explode crystal")]
    [SerializeField] private UI_skillTreeSlot unlockExplosiveButton;
    [SerializeField] private float ExplodeCooldown;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_skillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_skillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        if (unlockCrystalButton != null)
        {
            crystalUnlocked = unlockCrystalButton.unlocked;
            unlockCrystalButton.OnStateChanged += OnCrystalSlotChanged;
        }

        if (unlockCloneInstaedButton != null)
        {
            cloneInsteadOfCrystal = unlockCloneInstaedButton.unlocked;
            unlockCloneInstaedButton.OnStateChanged += OnCloneInsteadSlotChanged;
        }

        if (unlockExplosiveButton != null)
        {
            canExplode = unlockExplosiveButton.unlocked;
            if (canExplode) cooldown = ExplodeCooldown;
            unlockExplosiveButton.OnStateChanged += OnExplosiveSlotChanged;
        }

        if (unlockMovingCrystalButton != null)
        {
            canMoveToEnemy = unlockMovingCrystalButton.unlocked;
            unlockMovingCrystalButton.OnStateChanged += OnMovingSlotChanged;
        }

        if (unlockMultiStackButton != null)
        {
            canUseMultiStacks = unlockMultiStackButton.unlocked;
            unlockMultiStackButton.OnStateChanged += OnMultiStackSlotChanged;
            if (canUseMultiStacks) RefilCrystal();
        }
    }

    private void OnDestroy()
    {
        if (unlockCrystalButton != null) unlockCrystalButton.OnStateChanged -= OnCrystalSlotChanged;
        if (unlockCloneInstaedButton != null) unlockCloneInstaedButton.OnStateChanged -= OnCloneInsteadSlotChanged;
        if (unlockExplosiveButton != null) unlockExplosiveButton.OnStateChanged -= OnExplosiveSlotChanged;
        if (unlockMovingCrystalButton != null) unlockMovingCrystalButton.OnStateChanged -= OnMovingSlotChanged;
        if (unlockMultiStackButton != null) unlockMultiStackButton.OnStateChanged -= OnMultiStackSlotChanged;
    }

    public void CurrentCrystalChooseRandomTarget()
    {
        if (currentCrystal == null) return;
        currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ChooseRandomEnemy();
    }

    private void OnCrystalSlotChanged(UI_skillTreeSlot slot, bool newState) => crystalUnlocked = newState;
    private void OnCloneInsteadSlotChanged(UI_skillTreeSlot slot, bool newState) => cloneInsteadOfCrystal = newState;
    private void OnExplosiveSlotChanged(UI_skillTreeSlot slot, bool newState)
    {
        canExplode = newState;
        if (newState) cooldown = ExplodeCooldown;
    }
    private void OnMovingSlotChanged(UI_skillTreeSlot slot, bool newState) => canMoveToEnemy = newState;
    private void OnMultiStackSlotChanged(UI_skillTreeSlot slot, bool newState)
    {
        canUseMultiStacks = newState;
        if (!newState)
        {
            crystalLeft.Clear();
            if (currentCrystal != null) Destroy(currentCrystal);
            cooldownTimer = 0f;
        }
        else
        {
            RefilCrystal();
        }
    }

    public override bool CanUseSkill()
    {
        if (!crystalUnlocked)
        {
            player.fx.CreatePopUpText("locked");
            return false;
        }
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiStacks())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScirpt = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        currentCrystalScirpt.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    private bool CanUseMultiStacks()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if ((crystalLeft.Count == amountOfStacks))
                {
                    Invoke("ResetAbility", useTimeWindow);
                }
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>()
                    .SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }
        }
        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
