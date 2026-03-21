using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour ,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler                                 //物品格
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;

    protected UI ui;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }


    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        itemImage.color =Color.white;                                    //设为白色（显示图标）

        if (item != null)                                              //如果物品格里有物品
        {
            itemImage.sprite = item.data.icon;                             //图标

            if (item.stackSize > 1)                                          //如果数量大于1
            {
                itemText.text = item.stackSize.ToString();                     //显示数量
            }
            else
            {
                itemText.text = "";                                          //不显示数量
            }

        }
    }

    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;                                   //设为透明（不显示图标）
        itemText.text = "";                                              //不显示数量
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item==null)                                                 //预防点击空物品格报错
        {
            return;
        }

        ui.itemToolTip.HideToolTip();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }


        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipmentItem(item.data);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null||item.data==null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null||item.data==null)   
            return;
        ui.itemToolTip.HideToolTip();
    }
}
