using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop :ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()                                                       //生成掉落物
    {
        Inventory inventory=Inventory.instance;
        
        List<InventoryItem> itemsToUnequip=new List<InventoryItem>();
        List<InventoryItem> materialsToRemove=new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentList())                             //遍历装备栏
        {
            if (Random.Range(0,100)<=chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }
        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
            
        } 

        foreach(InventoryItem item in inventory.GetStashList())                                  //遍历材料栏
        {
            if (Random.Range(0,100)<=chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToRemove.Add(item);
            }
        }
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            inventory.RemoveItem(materialsToRemove[i].data);
        }



    }

}
