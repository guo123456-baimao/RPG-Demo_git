using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    public GameData gameData;
    private List<ISaveManager> saveManagers;                          //所有实现了ISaveManager接口的类的列表
    private FileDataHeadler dataHeadler;

    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        dataHeadler = new FileDataHeadler(Application.persistentDataPath, fileName,encryptData);
        dataHeadler.Delete();
    }


    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;


        dataHeadler = new FileDataHeadler(Application.persistentDataPath, fileName, encryptData);

        saveManagers = FindAllISaveManagers();

        LoadGame();

    }

    

    private void OnApplicationQuit()
    {
        SaveGame();
    }


    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHeadler.Load();

        if(gameData == null)
        {
            Debug.Log("No save data found");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)                         //遍历saveManagers,加载数据
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)                         //遍历saveManagers,保存数据
        {
            saveManager.SaveData(ref  gameData);
        }

        dataHeadler.Save(gameData);

    }


    private List<ISaveManager> FindAllISaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }


    public bool HasSaveData()
    {
        string fullPath = Path.Combine(dataHeadler.dataDirPath, dataHeadler.dataFileName);
        return File.Exists(fullPath);
    }

}
