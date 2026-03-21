using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHeadler
{
    public string dataDirPath = "";
    public string dataFileName = "";

    private bool encryptData=false;
    private string codeWord = "alexdev";

    public  FileDataHeadler(string _dataDirPath, string _dataFileName,bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        string fullPath=Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));                //创建目录

            string dataToStore=JsonUtility.ToJson(_data,true);                         //_data序列化

            if (encryptData)                                                           
                dataToStore = EncryptDecrypt(dataToStore);                            //加密数据



            using (FileStream stream=new FileStream(fullPath,FileMode.Create))             //打开文件
            {
                using(StreamWriter writer=new StreamWriter(stream))
                {
                    writer.Write(dataToStore);                                           //写入数据
                }
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error on try to saving data to file:" + fullPath + "\n" + e);
        }

    }

    public GameData Load()
    {
        string fullPath= Path.Combine(dataDirPath, dataFileName);
        GameData loadData=null;

        if (File.Exists(fullPath))                                                 //若文件存在
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))                            //打开文件
                {
                    using (StreamReader reader =new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();                                             //读取数据
                    }
                }

                if(encryptData)                                                                     //解密数据
                    dataToLoad = EncryptDecrypt(dataToLoad);


                loadData = JsonUtility.FromJson<GameData>(dataToLoad);                                   //反序列化
            }
            catch (Exception e)
            {
                Debug.Log("Error on try to load data from file:" + fullPath + "\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath)) 
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";
        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);                             //利用异或进行加密解密
        }

        return modifiedData;
    }


}
