using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem                                            //仓库物品（属性+数量）
{
    public ItemData data;                                             //属性
    public int stackSize;                                             //数量


    public InventoryItem(ItemData _data)                              
    {
        data = _data;
        AddStack();
    }

    public void AddStack()=> stackSize++;
    public void RemoveStack()=> stackSize--;

}
