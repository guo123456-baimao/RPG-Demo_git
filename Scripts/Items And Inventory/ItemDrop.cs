using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] posiblleDrop;
    private List<ItemData> dropList=new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;
        

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < posiblleDrop.Length; i++)                                                     //遍历可掉落物品数组
        {
            if (Random.Range(0, 100) <= posiblleDrop[i].dropChance)                                      //根据概率加到掉落列表
            {
                dropList.Add(posiblleDrop[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)                                                           //限制掉落件数
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];                              //打乱顺序

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }


    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity=new Vector2(Random.Range(-5,5), Random.Range(12,15));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }

}
