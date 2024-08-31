using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDatabase : MonoBehaviour
{

}

[System.Serializable]
public class Monster{
    public RaceType type;
    public int health;
    public int mana;
        
    public List<Ability> abilities = new List<Ability>();
}

[System.Serializable]
public class Ability{
    public string name;
    public string effect;
}