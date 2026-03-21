using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;

    private float _defaultAttackMultiplier;

    [Header("Clone attack")]
    [SerializeField] private UI_skillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_skillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Space]
    [Header("Multiple clone")]
    [SerializeField] private UI_skillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal Instead of Clone")]
    [SerializeField] private UI_skillTreeSlot crystalInseadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        _defaultAttackMultiplier = attackMultiplier;

        if (cloneAttackUnlockButton != null)
        {
            canAttack = cloneAttackUnlockButton.unlocked;
            if (canAttack) attackMultiplier = cloneAttackMultiplier;
            cloneAttackUnlockButton.OnStateChanged += OnCloneAttackChanged;
        }

        if (aggresiveCloneUnlockButton != null)
        {
            canApplyOnHitEffect = aggresiveCloneUnlockButton.unlocked;
            if (canApplyOnHitEffect) attackMultiplier = aggresiveCloneAttackMultiplier;
            aggresiveCloneUnlockButton.OnStateChanged += OnAggresiveCloneChanged;
        }

        if (multipleUnlockButton != null)
        {
            canDuplicateClone = multipleUnlockButton.unlocked;
            if (canDuplicateClone) attackMultiplier = multiCloneAttackMultiplier;
            multipleUnlockButton.OnStateChanged += OnMultipleCloneChanged;
        }

        if (crystalInseadUnlockButton != null)
        {
            crystalInsteadOfClone = crystalInseadUnlockButton.unlocked;
            crystalInseadUnlockButton.OnStateChanged += OnCrystalInsteadChanged;
        }
    }

    private void OnDestroy()
    {
        if (cloneAttackUnlockButton != null) cloneAttackUnlockButton.OnStateChanged -= OnCloneAttackChanged;
        if (aggresiveCloneUnlockButton != null) aggresiveCloneUnlockButton.OnStateChanged -= OnAggresiveCloneChanged;
        if (multipleUnlockButton != null) multipleUnlockButton.OnStateChanged -= OnMultipleCloneChanged;
        if (crystalInseadUnlockButton != null) crystalInseadUnlockButton.OnStateChanged -= OnCrystalInsteadChanged;
    }

    private void OnCloneAttackChanged(UI_skillTreeSlot slot, bool newState)
    {
        canAttack = newState;
        attackMultiplier = newState ? cloneAttackMultiplier : _defaultAttackMultiplier;
    }

    private void OnAggresiveCloneChanged(UI_skillTreeSlot slot, bool newState)
    {
        canApplyOnHitEffect = newState;
        attackMultiplier = newState ? aggresiveCloneAttackMultiplier : _defaultAttackMultiplier;
    }

    private void OnMultipleCloneChanged(UI_skillTreeSlot slot, bool newState)
    {
        canDuplicateClone = newState;
        attackMultiplier = newState ? multiCloneAttackMultiplier : _defaultAttackMultiplier;
    }

    private void OnCrystalInsteadChanged(UI_skillTreeSlot slot, bool newState) => crystalInsteadOfClone = newState;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(_clonePosition),canDuplicateClone,chanceToDuplicate,player,attackMultiplier);
        //我这里把FindclosestEnemy(newClone.Transform)改了
    }

    

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(1 * player.facingDir, 0, 0)));
        
    }

    private IEnumerator CloneDelayCorotine(Transform _transform,Vector3 _offset) 
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform,_offset);
    }
}
