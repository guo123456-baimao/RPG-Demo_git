using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_skillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnclock {  get; private set; }

    [Header("Mirage Dodge")]
    [SerializeField] private UI_skillTreeSlot unlockMirageDodgeButton;
    public bool dodgeMirageUnlock {  get; private set; }

    protected override void Start()
    {
        base.Start();

        if (unlockDodgeButton != null)
        {
            dodgeUnclock = unlockDodgeButton.unlocked;
            if (dodgeUnclock)
            {
                if (player.stats != null)
                {
                    player.stats.evasion.AddModifier(evasionAmount);
                    Inventory.instance.UpdateStatsUI();
                }    
            }
            unlockDodgeButton.OnStateChanged += OnDodgeSlotChanged;
        }

        if (unlockMirageDodgeButton != null)
        {
            dodgeMirageUnlock = unlockMirageDodgeButton.unlocked;
            unlockMirageDodgeButton.OnStateChanged += OnMirageDodgeSlotChanged;
        }
    }

    private void OnDestroy()
    {
        if (unlockDodgeButton != null) unlockDodgeButton.OnStateChanged -= OnDodgeSlotChanged;
        if (unlockMirageDodgeButton != null) unlockMirageDodgeButton.OnStateChanged -= OnMirageDodgeSlotChanged;
    }

    private void OnDodgeSlotChanged(UI_skillTreeSlot slot, bool newState)
    {
        if (newState && !dodgeUnclock)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnclock = true;
        }
        else if (!newState && dodgeUnclock)
        {
            player.stats.evasion.RemoveModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnclock = false;
        }
    }

    private void OnMirageDodgeSlotChanged(UI_skillTreeSlot slot, bool newState) => dodgeMirageUnlock = newState;

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlock)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
    }
}
