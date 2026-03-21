using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot :UI_ItemSlot
{
    public EquipmentType slotType;                              //装备类型

    private void OnValidate()
    {
        gameObject.name="equipment Slot - "+slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item==null||item.data==null)                                            //预防点击空武器格报错
            return;
        
        ui.itemToolTip.HideToolTip();

        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);


        CleanUpSlot();

    }

}
