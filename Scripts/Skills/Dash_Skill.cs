using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]
    [SerializeField] private UI_skillTreeSlot dashUnlockButton;
    public bool dashUnlocked {  get; private set; }

    [Header("Clone on dash")]
    [SerializeField] private UI_skillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked {  get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private UI_skillTreeSlot cloneOnArrivalUnlockButton;
    public bool cloneOnArrivalUnlocked {  get; private set; }

    protected override void Start()
    {
        base.Start();

        if (dashUnlockButton != null)
        {
            dashUnlocked = dashUnlockButton.unlocked;
            dashUnlockButton.OnStateChanged += OnDashSlotChanged;
        }

        if (cloneOnDashUnlockButton != null)
        {
            cloneOnDashUnlocked = cloneOnDashUnlockButton.unlocked;
            cloneOnDashUnlockButton.OnStateChanged += OnCloneOnDashSlotChanged;
        }

        if (cloneOnArrivalUnlockButton != null)
        {
            cloneOnArrivalUnlocked = cloneOnArrivalUnlockButton.unlocked;
            cloneOnArrivalUnlockButton.OnStateChanged += OnCloneOnArrivalSlotChanged;
        }
    }

    private void OnDestroy()
    {
        if (dashUnlockButton != null) dashUnlockButton.OnStateChanged -= OnDashSlotChanged;
        if (cloneOnDashUnlockButton != null) cloneOnDashUnlockButton.OnStateChanged -= OnCloneOnDashSlotChanged;
        if (cloneOnArrivalUnlockButton != null) cloneOnArrivalUnlockButton.OnStateChanged -= OnCloneOnArrivalSlotChanged;
    }

    private void OnDashSlotChanged(UI_skillTreeSlot slot, bool newState) => dashUnlocked = newState;
    private void OnCloneOnDashSlotChanged(UI_skillTreeSlot slot, bool newState) => cloneOnDashUnlocked = newState;
    private void OnCloneOnArrivalSlotChanged(UI_skillTreeSlot slot, bool newState) => cloneOnArrivalUnlocked = newState;

    public override void UseSkill()
    {
        base.UseSkill();
    }

    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
}
