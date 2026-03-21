using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_craftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();                             //清除之前绑定的事件

        for (int i = 0; i < materialImage.Length; i++)                                  //遍历材料图片
        {
            materialImage[i].color = Color.clear;                                                      //图片的颜色清除
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color=Color.clear;             //图片的子级文本颜色也清除
        }

        for (int i = 0; i < _data.craftingMaterials.Count; i++)                          //把武器所需材料投影到materialImage栏
        {
            if (_data.craftingMaterials.Count>materialImage.Length)
                Debug.LogWarning("you have more material amount than you hava material slots in craft window ");
            
            materialImage[i].sprite = _data.craftingMaterials[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        itemIcon.sprite = _data.icon;                                                        //其他武器数据投影进craftwindow
        itemName.text =_data.name;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(()=>Inventory.instance.CanCraft(_data,_data.craftingMaterials));               //添加监听事件
    }

}
