using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [SerializeField] private UI_skillTreeSlot blackHoleUnlockButton;
    public bool blackholeUnlocked {  get; private set; }
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    protected override void Start()
    {
        base.Start();

        if (blackHoleUnlockButton != null)
        {
            blackholeUnlocked = blackHoleUnlockButton.unlocked;
            blackHoleUnlockButton.OnStateChanged += OnBlackholeSlotChanged;
        }
    }

    private void OnDestroy()
    {
        if (blackHoleUnlockButton != null) blackHoleUnlockButton.OnStateChanged -= OnBlackholeSlotChanged;
    }

    private void OnBlackholeSlotChanged(UI_skillTreeSlot slot, bool newState) => blackholeUnlocked = newState;

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);
        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();
        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration);

        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole) return false;
        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }

    public float GetBlackholeRadius() => maxSize / 2;
}
