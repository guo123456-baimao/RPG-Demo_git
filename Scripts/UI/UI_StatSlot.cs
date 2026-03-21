using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;                                    //状态名
    [SerializeField] private StatType statType;                                  //状态类
    [SerializeField] private TextMeshProUGUI statValueText;                      //状态值-UI组件
    [SerializeField] private TextMeshProUGUI statNameText;                       //状态名-UI组件

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat-" + statName;
        if (statNameText!=null)
        {
            statNameText.text = statName;
        }
    }

    void Start()
    {
        UpdataStatValueUI();
        ui=GetComponentInParent<UI>();
    }

    public void UpdataStatValueUI()
    {
        PlayerStats playerStats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats!=null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
        }

        if (statType==StatType.health)
            statValueText.text = playerStats.GetMaxHealthValue().ToString();
        if (statType==StatType.damaged)
            statValueText.text=(playerStats.damaged.GetValue()+playerStats.strength.GetValue()).ToString();
        if(statType==StatType.critPower)
            statValueText.text=(playerStats.critPower.GetValue()+playerStats.strength.GetValue()).ToString();
        if (statType == StatType.critChance)
            statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();
        if (statType == StatType.evasion)
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
        if (statType == StatType.magicRes)
            statValueText.text = (playerStats.magicResisence.GetValue() + (playerStats.intelligence.GetValue()*3)).ToString();


    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }

}
