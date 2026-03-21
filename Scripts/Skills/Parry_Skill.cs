using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_skillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_skillTreeSlot restoreUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked {  get; private set; }

    [Header("Parry with mirage")]
    [SerializeField] private UI_skillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        if (parryUnlockButton != null)
        {
            parryUnlocked = parryUnlockButton.unlocked;
            parryUnlockButton.OnStateChanged += OnParrySlotChanged;
        }

        if (restoreUnlockButton != null)
        {
            restoreUnlocked = restoreUnlockButton.unlocked;
            restoreUnlockButton.OnStateChanged += OnRestoreSlotChanged;
        }

        if (parryWithMirageUnlockButton != null)
        {
            parryWithMirageUnlocked = parryWithMirageUnlockButton.unlocked;
            parryWithMirageUnlockButton.OnStateChanged += OnParryWithMirageSlotChanged;
        }
    }

    private void OnDestroy()
    {
        if (parryUnlockButton != null) parryUnlockButton.OnStateChanged -= OnParrySlotChanged;
        if (restoreUnlockButton != null) restoreUnlockButton.OnStateChanged -= OnRestoreSlotChanged;
        if (parryWithMirageUnlockButton != null) parryWithMirageUnlockButton.OnStateChanged -= OnParryWithMirageSlotChanged;
    }

    private void OnParrySlotChanged(UI_skillTreeSlot slot, bool newState) => parryUnlocked = newState;
    private void OnRestoreSlotChanged(UI_skillTreeSlot slot, bool newState) => restoreUnlocked = newState;
    private void OnParryWithMirageSlotChanged(UI_skillTreeSlot slot, bool newState) => parryWithMirageUnlocked = newState;

    public override bool CanUseSkill()
    {
        return parryUnlocked && base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restoreHealthPercentage);
            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }
}
