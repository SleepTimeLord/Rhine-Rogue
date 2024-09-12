using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{

    void Start(){
        LoadJson();
    }

    public PlayerData playerData = new PlayerData();
    public PlayerSettings playerSettings = new PlayerSettings();

    public void SaveJson()
    {
        string plrData = JsonUtility.ToJson(playerData);
        string settingsData = JsonUtility.ToJson(playerSettings);
        string DataFilePath = Application.persistentDataPath + "/PlayerData.json";
        string SettingsFilePath = Application.persistentDataPath + "/PlayerSettings.json";
        System.IO.File.WriteAllText(DataFilePath,plrData);
        System.IO.File.WriteAllText(SettingsFilePath,settingsData);
        Debug.Log("Saved to" + Application.persistentDataPath);
    }

    public void LoadJson()
    {
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        string plrData = System.IO.File.ReadAllText(filePath);

        //Add settings later !!!!
        Debug.Log(plrData);
    }

}

[System.Serializable]
public class PlayerData{
    public int coins;
    public int health;
    public List<Monster> party = new List<Monster>();
}

public class PlayerSettings{
    public int masterVolume;
    public int musicVolume;
    public int sfxVolume;
    public int graphicsIndex;
    public int antialiasingIndex;
    public float renderScale;
    public int resolutionIndex;
    public Boolean vSync;
    public Boolean fullScreen;
}



