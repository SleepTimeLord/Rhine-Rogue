using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{
    public void LoadJson()
    {
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        string plrData = System.IO.File.ReadAllText(filePath);

        Debug.Log(plrData);
    }
}

[System.Serializable]
public class Monster{
    public int tag;
    public RaceType type;
    public int health;
    public int mana;
    public int attack;
    public int defense;
    public List<Passive> passives = new List<Passive>();
    public List<Ability> abilities = new List<Ability>();
}

[System.Serializable]
public class Ability{
    public Weapon weapon;
}

public class Passive{
    public string name;
}
