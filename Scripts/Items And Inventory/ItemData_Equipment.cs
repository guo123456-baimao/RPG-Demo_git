using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,                  //武器
    Armor,                   //护甲
    Amulet,                  //护身符
    Flask                    //药水

}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]

public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects = new ItemEffect[0];          // 防止为 null，给默认空数组


    [Header("Major stats")]
    public int strength;                     //强度
    public int agility;                      //敏捷
    public int intelligence;                 //智力
    public int vitality;                     //体力

    [Header("Offensive stats")]
    public int damaged;                      //伤害
    public int critChance;                   //暴击率
    public int critPower;                    //暴击强度


    [Header("Defensive stats")]
    public int maxHealth;                     //最大生命
    public int armor;                         //护甲
    public int evasion;                       //闪避
    public int magicResisence;               //魔法抗性

    [Header("Magic stats")]
    public int fireDamage;                 //火焰伤害  
    public int iceDamage;                  //冰霜伤害
    public int lightningDamage;            //闪电伤害

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;
    
    public void AddModifiers()
    {
        PlayerStats playerStats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damaged.AddModifier(damaged);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResisence.AddModifier(magicResisence);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damaged.RemoveModifier(damaged);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);
        
        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResisence.RemoveModifier(magicResisence);
        
        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightningDamage);
    }

    public void Effect(Transform _enemyPosition)
    {
        if (itemEffects == null) return;
        foreach(var item in itemEffects)                                  //遍历特效词条数组
        {
            if (item == null) continue;
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public override string GetDescription()
    {
        // 兜底：确保 sb 已初始化（ItemData 中通常已初始化，但加防护）
        if (sb == null) sb = new System.Text.StringBuilder();

        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "strength");
        AddItemDescription(agility, "agility");
        AddItemDescription(intelligence, "intelligence");
        AddItemDescription(vitality, "vitality");

        AddItemDescription(damaged, "damaged");
        AddItemDescription(critChance, "critChance");
        AddItemDescription(critPower, "critPower");

        AddItemDescription(maxHealth, "maxHealth");
        AddItemDescription(armor, "armor");
        AddItemDescription(evasion, "evasion");
        AddItemDescription(magicResisence, "magicResisence");

        AddItemDescription(fireDamage, "fireDamage");
        AddItemDescription(iceDamage, "iceDamage");
        AddItemDescription(lightningDamage, "lightningDamage");

        if (itemEffects != null)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                var effect = itemEffects[i];
                if (effect == null) continue;
                if (!string.IsNullOrEmpty(effect.effectDescription))
                {
                    sb.AppendLine();
                    sb.AppendLine("Unique: " + effect.effectDescription);
                    descriptionLength++;
                }
            }
        }

        if (descriptionLength < 5)
        {
            for (int i = 0;i < 5-descriptionLength;i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value,string _name)
    {
        if (_value!=0)
        {
            if (sb == null) sb = new System.Text.StringBuilder();
            if (sb.Length>0)
            {
                sb.AppendLine();
            }
            if (_value>0)
            {
                sb.Append("+" + _value + " " + _name);
            }
            else
            {
                sb.Append(_value + " " + _name);
            }
            descriptionLength++;
        }
    }

}
