using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_craftList : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;
   
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_craftList>().SetupCraftList();                   //调用并展示第一个列表
        SetupDefaultCraftWindow();                                                                    //显示第一个武器的数据
    }

   

    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)                                    //遍历列表，并销毁
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);                         
        }

        for (int i = 0; i < craftEquipment.Count; i++)                                               //遍历武器列表
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);                  //生成craftSlot预制体
            newSlot.GetComponent<UI_craftSlot>().SetupCraftSlot(craftEquipment[i]);              //给craftSlot预制体赋予属性
            
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0]!=null)
        {
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
        }
    }

}
