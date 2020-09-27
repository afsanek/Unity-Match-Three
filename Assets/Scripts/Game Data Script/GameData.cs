using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class DataSaver
{
    public bool[] isActive;
    public int[] HighScore;
    public int[] EarnedStars;
    public int lastActiveLevel;
}
public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public DataSaver dataSaver;
    private string path;

    private void Awake()
    {
        path = Application.persistentDataPath + "/PlayerData.gd";
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }
    public void Save()
    {
       // PlayerPrefs.SetString("Data",);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = new FileStream(path, FileMode.OpenOrCreate);
        DataSaver data = new DataSaver();
        data = dataSaver;
        formatter.Serialize(file, data);
        file.Flush();
        file.Close();
        //print("Save");
        
    }
    public void CreateSave()
    {
        dataSaver = new DataSaver
        {
            EarnedStars = new int[153],
            HighScore = new int[153],
            isActive = new bool[153],
            lastActiveLevel = 0
        };
        dataSaver.isActive[0] = true;
        Save();
    }
    public void OnApplicationQuit()
    {
        Save();
    }
    void OnDisable()
    {
        Save();
    }
    
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }
    
    public void Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = new FileStream(path, FileMode.Open);
            dataSaver = formatter.Deserialize(file) as DataSaver;
            file.Close();
           // print("load");
        }
        else
        {
            CreateSave();
        }
    }
}
