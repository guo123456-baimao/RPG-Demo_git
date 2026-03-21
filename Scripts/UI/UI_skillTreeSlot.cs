using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_skillTreeSlot : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler,ISaveManager
{
    public UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescriptiom;
    [SerializeField] private Color lockedSkillColor;

    [SerializeField] private bool _unlocked; // backing field for inspector
    public bool unlocked { get => _unlocked; set => SetUnlocked(value); }

    // 订阅事件：(slot, newState)
    public event Action<UI_skillTreeSlot, bool> OnStateChanged;

    [SerializeField] private UI_skillTreeSlot[] shouldBeUnlocked;                       //应解锁的列表
    [SerializeField] private UI_skillTreeSlot[] shouldBeLocked;                         //应锁上的列表(同级）
    [SerializeField] private UI_skillTreeSlot[] nextBeLocked;                           //后续应锁上的列表（下级）

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=>UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        if (skillImage != null)
            skillImage.color = _unlocked ? Color.white : lockedSkillColor;
    }

    // 统一入口修改状态，更新 UI 并通知订阅者
    public void SetUnlocked(bool value, bool notify = true)
    {
        if (_unlocked == value) return;

        _unlocked = value;

        if (skillImage != null)
            skillImage.color = _unlocked ? Color.white : lockedSkillColor;

        if (notify)
            OnStateChanged?.Invoke(this, _unlocked);
    }

    public void UnlockSkillSlot()
    {
        if (_unlocked)
            return;
        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        if (shouldBeUnlocked != null)
        {
            for (int i = 0; i < shouldBeUnlocked.Length; i++)
            {
                if (shouldBeUnlocked[i] != null && shouldBeUnlocked[i].unlocked == false) // 依赖未解锁
                {
                    Debug.Log("cannot unlock skill");
                    return;
                }
            }
        }

        if (shouldBeLocked != null)
        {
            for (int i = 0; i < shouldBeLocked.Length; i++)
            {
                if (shouldBeLocked[i] != null && shouldBeLocked[i].unlocked == true) // 禁止解锁的已解锁
                {
                    Debug.Log("cannot unlock skill");
                    return;
                }
            }
        }

        SetUnlocked(true);
    }

    public void LockAndLoacknext()
    {
        // 自己与下级都走 SetUnlocked（会触发事件）
        SetUnlocked(false);

        if (nextBeLocked != null)
        {
            foreach (UI_skillTreeSlot slot in nextBeLocked)
            {
                if (slot == null) continue;
                slot.SetUnlocked(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescriptiom,skillName,skillCost);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) //右键锁上技能
        {
            LockAndLoacknext();
        }
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName,out bool value))
        {
            SetUnlocked(value);
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, _unlocked);
        }
        else
            _data.skillTree.Add(skillName, _unlocked);
    }
}
