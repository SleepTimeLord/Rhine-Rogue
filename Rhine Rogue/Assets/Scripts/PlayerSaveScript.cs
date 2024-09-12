using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();

    public void SaveJson()
    {
        string plrData = JsonUtility.ToJson(playerData);
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        System.IO.File.WriteAllText(filePath,plrData);
        Debug.Log("Saved to" + Application.persistentDataPath);
    }

    public void LoadJson()
    {
        
    }

}

[System.Serializable]
public class PlayerData{
    public int coins;
    public int health;
    public List<Monster> party = new List<Monster>();
}



