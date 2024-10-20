using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{

    public PlayerData playerData = new PlayerData();
    public PlayerSettings playerSettings = new PlayerSettings();

    //JSON PATHS
    string dataFilePath;
    string settingsFilePath;

    void Start()
    {
        //LoadJson();

        dataFilePath = Application.persistentDataPath + "/PlayerData.json";
        settingsFilePath = Application.persistentDataPath + "/PlayerSettings.json";
    }

    public void SaveJson()
    {
        string plrData = JsonUtility.ToJson(playerData);
        string settingsData = JsonUtility.ToJson(playerSettings);

        System.IO.File.WriteAllText(dataFilePath,plrData);
        System.IO.File.WriteAllText(settingsFilePath,settingsData);

        Debug.Log("Saved to" + Application.persistentDataPath);
    }

    public void LoadJson()
    {
        string plrData = System.IO.File.ReadAllText(dataFilePath);
        string settingsData = System.IO.File.ReadAllText(settingsFilePath);

        Debug.Log(plrData);
        Debug.Log(settingsData);
    }

}

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int health;
    public List<Entity> party = new List<Entity>();
}

public class PlayerSettings
{
    public int masterVolume;
    public int musicVolume;
    public int sfxVolume;
    public int graphicsIndex;
    public int antialiasingIndex;
    public float renderScale;
    public int resolutionIndex;
    public bool vSync;
    public bool fullScreen;
}


