using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class Stat 
{
    public List<int> modifiers;      //ÐÞ¸ÄÆ÷

    [SerializeField]private int baseValue;

    public int GetValue()
    {
        int finalValue=baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue=_value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }



}
