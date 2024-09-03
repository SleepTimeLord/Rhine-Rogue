using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{

    public List<Monster> monsterStorage = new List<Monster>();
    public void SaveDatabase()
    {
        string monsterdata = JsonUtility.ToJson(monsterStorage);
        string filePath = Application.persistentDataPath + "/MonsterDatabase.json";
        System.IO.File.WriteAllText(filePath, monsterdata);
        Debug.Log("Saved to" + Application.persistentDataPath);
    }

}

[System.Serializable]
public class Monster{
    public RaceType type;
    public int health;
    public int mana;
    public int defense;
    public int attack;
    public List<StatusEffect> statuses = new List<StatusEffect>();
    public List<Ability> abilities = new List<Ability>();
}

[System.Serializable]
public class Ability{
    public string name;
    public string effect;
}