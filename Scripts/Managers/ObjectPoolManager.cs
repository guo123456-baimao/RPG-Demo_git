using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance { get; private set; }
    
    private Transform poolParent;
    private Dictionary<GameObject,Stack<GameObject>> poolDic = new Dictionary<GameObject, Stack<GameObject>>();

    public GameObject slime_BallOfIce_Prefab;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CreatePoolParent();
    }

    private void Start()
    {
        if (instance != null)
        PreWarmPool(slime_BallOfIce_Prefab,6);                     //‘§»»Slime_BallOfIce∂‘œÛ≥ÿ£¨
    }

    private void CreatePoolParent()
    {
        GameObject poolParentObj = new GameObject("PoolParent");
        poolParent = poolParentObj.transform;
        poolParent.SetParent(this.transform);
    }

    public GameObject GetObj(GameObject prefab)
    {
        GameObject targetObj = null;

        if (!poolDic.ContainsKey(prefab))
        {
            poolDic.Add(prefab, new Stack<GameObject>());
        }

        if (poolDic[prefab].Count>0)
        {
            targetObj = poolDic[prefab].Pop();
            targetObj.transform.rotation = Quaternion.identity;              //
        }
        else
        {
            targetObj= Instantiate(prefab);
            poolDic[prefab].Push(targetObj);
            targetObj.SetActive(false);
            targetObj.transform.SetParent(poolParent);
            targetObj.transform.rotation = Quaternion.identity;             //
            targetObj = poolDic[prefab].Pop();
        }

        if (!targetObj.activeSelf)
        {
            targetObj.SetActive(true);
            
        }

        return targetObj;
    }

    public void RecycleObj(GameObject prefab,GameObject obj)
    {
        if(obj==null||!poolDic.ContainsKey(prefab))
        {
            return;
        }

        if (obj.activeSelf)
        {
            obj.SetActive(false);
            
        }

        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        if (!poolDic.ContainsKey(prefab))
        {
            poolDic.Add(prefab, new Stack<GameObject>());
        }

        obj.transform.SetParent(poolParent);
        poolDic[prefab].Push(obj);
    }

    public void PreWarmPool(GameObject prefab,int count)
    {
        if (prefab==null||count<=0)
        {
            return;
        }

        if (!poolDic.ContainsKey(prefab))
        {
            poolDic.Add(prefab, new Stack<GameObject>());
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            poolDic[prefab].Push(obj);
        }
    }

}
