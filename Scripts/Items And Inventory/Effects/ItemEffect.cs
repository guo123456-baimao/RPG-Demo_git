using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemEffect : ScriptableObject                               //词条
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)                                  //执行特效
    {
        Debug.Log("Effect executed");
    }
}
