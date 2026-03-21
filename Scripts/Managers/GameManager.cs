using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,ISaveManager
{
    public static GameManager instence;

    private Transform player;                                     //!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    private void Awake()
    {
        if(instence!=null)
            Destroy(instence.gameObject);
        else
            instence = this;

        checkPoints=FindObjectsOfType<CheckPoint>();                              //查找场景中CheckPoint的组件
    }

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }



    public void RestartScene()
    {
        Scene scene=SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        closestCheckpointId = _data.closestCheckpointId;

        //Invoke("PlacePlayerAtClosestCheckpoint", .1f);                                           //玩家切换位置到最近的点亮检查点

        StartCoroutine(LoadWithDelay(_data));
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)                           //遍历存档的检查点字典
        {
            foreach (CheckPoint checkPoint in checkPoints)                                   //遍历检查点列表          
            {
                if (checkPoint.id == pair.Key && pair.Value == true)                              //如果匹配成功且有应该亮着的检查点
                {
                    checkPoint.ActivatePoint();                                                 //检查点设为活跃
                }
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount>0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
            
        }
       

        lostCurrencyAmount = 0;

    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint();

        yield return new WaitForSeconds(.5f);
        LoadLostCurrency(_data);
    }




    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount=lostCurrencyAmount;
        _data.lostCurrencyX= player.position.x;
        _data.lostCurrencyY= player.position.y;

        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;            //找到最近的点亮的检查点Id
        else
            _data.closestCheckpointId = null;
        _data.checkpoints.Clear();                                    //清空存档的检查点字典

        foreach(CheckPoint checkPoint in checkPoints)                    //遍历检查点列表
        {
            _data.checkpoints.Add(checkPoint.id, checkPoint.activationStatus);                     //存档的检查点字典添加检查点
        }  
    }

    private void LoadClosestCheckpoint()
    {
        foreach (CheckPoint checkPoint in checkPoints)                              //遍历检查点列表
        {
            if (closestCheckpointId== checkPoint.id)                                  //如果有最近的点亮检查点
                PlayerManager.instance.player.transform.position = checkPoint.transform.position;                 //玩家切换位置到检查点
        }
    }

    private CheckPoint FindClosestCheckpoint()                                     //找到最近的点亮的检查点
    {
        float closestDistence = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        foreach (var checkpoint in checkPoints)
        {
            float distenceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);

            if (distenceToCheckpoint<closestDistence&&checkpoint.activationStatus==true)
            {
                closestDistence = distenceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;

    }

    public void PauseGame(bool _pause)                                        //时停
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

}
