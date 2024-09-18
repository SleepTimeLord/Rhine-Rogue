using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    public List<GameObject> allMonsters;

    //Can separate diff sections for diff regions that certain monsters spawn in

    public GameObject LoadRandomMonster(){
        return allMonsters[Random.Range(0, allMonsters.Count)];
    }

}
