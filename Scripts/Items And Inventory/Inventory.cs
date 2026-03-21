using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour,ISaveManager                                         //仓库
{
    public static Inventory instance;                                                //单例

    public List<ItemData> startingItems;                                         //初始物品列表 

    public List<InventoryItem> equipment;                                                   //装备列表
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;               //装备字典《属性，属性+数量》

    public List<InventoryItem> inventory;                                        //物品栏列表
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;              //物品栏字典《属性，属性+数量》

    public List<InventoryItem> stash;                                            //储藏栏
    public Dictionary<ItemData, InventoryItem> stashDictionary;                  //储藏栏字典《属性，属性+数量》

    [Header("Iventory UI")]
    [SerializeField] private Transform inventorySlotParent;                      //UI物品栏父物体
    [SerializeField] private Transform stashSlotParent;                          //UI储藏栏父物体
    [SerializeField] private Transform equipmentSlotParent;                      //UI装备栏父物体
    [SerializeField] private Transform statSlotParent;                           //UI状态栏父物体

    private UI_ItemSlot[] inventoryItemSlots;                                       //UI物品栏数组
    private UI_ItemSlot[] stashItemSlots;                                           //UI储藏栏数组
    private UI_EquipmentSlot[] equipmentItemSlots;                                  //UI装备栏数组
    private UI_StatSlot[] statSlot;                                                 //UI状态栏数组

    [Header("Items cooldown")]
    private float lastTimeUsedFlasked;
    private float lastTimeUsedArmor;

    public float flaskCooldown {  get; private set; }
    private float armorCooldown;

    [Header("Data base")]
    public List<ItemData> itemDataBase;                        //物品数据列表
    public List<InventoryItem> loadedItems;                    //加载物品列表
    public List<ItemData_Equipment> loadedEquipment;             //加载装备列表


    private void Awake()
    {
        if (instance==null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();


        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
        

        AddStartingItems();

    }

    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)                                  //加载装备栏
        {
            EquipmentItem(item);
        }


        if (loadedItems.Count>0)                                                              //有存档就加载存档
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i=0; i<item.stackSize;i++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }

        for (int i = 0; i < startingItems.Count; i++)                                        //没存档才加载初始数据
        {
            if(startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    public void EquipmentItem(ItemData _item)                                                        
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;                               //把ItemData转换成ItemData_Equipment，点击的物品
        InventoryItem newItem = new InventoryItem(newEquipment);                                     //创建一个新的属性+数量

        ItemData_Equipment oldEquipment = null;                                                      //要移除的物品

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)        //遍历武器字典
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)                                //如果字典里有相同类型的装备
            {
                oldEquipment = item.Key;                                                             //把字典这个物品赋值给itemToRemove
            }
        }
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);                                                                //卸下这个物品
            AddItem(oldEquipment);                                                                    //把这个物品放回物品栏
        }
        equipment.Add(newItem);                                                                       //添加到武器列表
        equipmentDictionary.Add(newEquipment, newItem);                                               //添加到武器字典
        newEquipment.AddModifiers();                                                                  //添加属性
        RemoveItem(_item);                                                                            //从物品栏移除这个物品

        UpdateSlotUI();                                                                             //更新UI  
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)                                      //卸下这个物品
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);                                                                 //从武器列表移除
            equipmentDictionary.Remove(itemToRemove);                                                //从武器字典移除
            itemToRemove.RemoveModifiers();                                                          //移除属性
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }
        for (int i = 0; i < stashItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }



        for (int i = 0; i < equipmentItemSlots.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)        //遍历武器字典
            {
                if (item.Key.equipmentType == equipmentItemSlots[i].slotType)                           //如果字典里有相同类型的装备
                {
                    equipmentItemSlots[i].UpdateSlot(item.Value);                                        //更新这个物品格
                }
            }
        }
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();

    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdataStatValueUI();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType==ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if (_item.itemType==ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateSlotUI();                                                                     //更新UI
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))                 //如果字典里有这个属性
        {
            value.AddStack();
        }
        else                                                                                //如果字典里没有这个属性
        {
            InventoryItem newItem = new InventoryItem(_item);                                //创建一个新的属性+数量
            inventory.Add(newItem);                                                    //添加到列表
            inventoryDictionary.Add(_item, newItem);                                         //添加到字典
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))             //如果字典里有这个属性
        {
            if (value.stackSize <= 1)                                                    //如果数量小于等于1
            {
                inventory.Remove(value);                                            //从列表移除
                inventoryDictionary.Remove(_item);                                       //从字典移除
            }
            else                                                                         //如果数量大于1
            {    
                value.RemoveStack();                                                     //数量减1
            }
        }

        if (stashDictionary.TryGetValue(_item,out InventoryItem stashValue))                //如果字典里有这个属性
        {
            if (stashValue.stackSize<=1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }



        UpdateSlotUI();                                                                 //更新UI  
    }

    public bool CanAddItem()
    {
        if (inventory.Count>=inventoryItemSlots.Length)
        {
            Debug.Log("No more space");
            return false;
        }
        return true;
    }


    public bool CanCraft(ItemData_Equipment _itemToCraft,List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();                             //创建一个新的列表，存储要移除的材料

        for (int i = 0; i < _requiredMaterials.Count; i++)                                                 //遍历所需材料
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data,out InventoryItem stashValue))                   //如果stash字典里有这个属性
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)                                      //如果字典里的数量小于所需数量
                {
                    Debug.Log("Not enough materials");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);                                                      //把这个材料添加到要移除的列表
                }
            }
            else
            {
                Debug.Log("Not enough materials"); 
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)                                    //遍历要移除的材料
        {
            for (int a = 0; a<_requiredMaterials[i].stackSize; a++)                                //根据数量移除材料
            {
                RemoveItem(materialsToRemove[i].data);                                           //从stash仓库移除这个材料
            }
        }

        AddItem(_itemToCraft);                                                              //把合成好的物品添加到物品栏
        Debug.Log ("this is your item"+_itemToCraft.name);
        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)                            //通过类型获取装备
    {
        ItemData_Equipment equipmentItem = null;
        foreach (KeyValuePair<ItemData_Equipment,InventoryItem> item in equipmentDictionary)           //遍历武器栏
        {
            if (item.Key.equipmentType==_type)
            {
                equipmentItem = item.Key;
            }
        }
        return equipmentItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask=GetEquipment(EquipmentType.Flask);

        if (currentFlask==null)
        {
            return;
        }

        bool canUseFlask = Time.time>lastTimeUsedFlasked+flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlasked = Time.time;
        }
        else
        {
            Debug.Log("flask on cooldown");
        }
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor=GetEquipment(EquipmentType.Armor);

        if (Time.time>lastTimeUsedArmor+armorCooldown)
        {
            armorCooldown= currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }
        Debug.Log("Armor on cooldown");
        return false;

    }

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string,int> pair in _data.inventory)                                    //遍历存档的inventory
        {
            foreach (var item in itemDataBase)                                                  //获取资源里所有item数据
            {
                if (item != null && item.itemId == pair.Key)                                         //匹配GUID
                {
                    InventoryItem itemToLoad=new InventoryItem (item);
                    itemToLoad.stackSize=pair.Value;

                    loadedItems.Add(itemToLoad);                                                     //添加到loadedItems数组
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)                                           //遍历存档的equipmentId列表
        {
            foreach (var item in itemDataBase)                                                  //获取资源里所有item数据
            {
                if (item!=null&&loadedItemId==item.itemId)                                           //匹配GUID
                {
                    loadedEquipment.Add(item as ItemData_Equipment);                                 //添加到loadedEquipment数组
                }
            }
        }



    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();                                        //清除上次存档inventory的数据
        _data.equipmentId.Clear();                                      //清除上次存档equipment的数据

        foreach (KeyValuePair<ItemData,InventoryItem> pair in inventoryDictionary)              //遍历inventoryDictionary字典
        {
            _data.inventory.Add(pair.Key.itemId,pair.Value.stackSize);                           // _data的inventory字典添加数据
        }
        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)              //遍历stashDictionary字典
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);                           // _data的inventory字典添加数据
        }
        foreach(KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)               //遍历equipmentDictionary字典
        {
            _data.equipmentId.Add(pair.Key.itemId);                                             //_data的equipmentId列表添加数据
        }

    }


#if UNITY_EDITOR

    [ContextMenu("Fill up item data base")]
    private void FillupItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Date/Items" });          //找到该资源路径下的GUID数组

        foreach (string SOName in assetNames)                                                   //遍历GUID数组 
        {
            var SOPath = AssetDatabase.GUIDToAssetPath(SOName);                              //返回资源路径
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOPath);                  //返回资源
            itemDataBase.Add(itemData);                                                      //数据库添加资源
        }
        return itemDataBase;                                                                 //返回Equipment下的SO资源数组
    }

#endif
    

}
