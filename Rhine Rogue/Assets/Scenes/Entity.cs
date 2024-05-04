using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public float health;
    public Vector3Int position;
    public RaceType raceType;
    public int speed;
    public int initiative;
    public int baseInitiative;
    public float attack;
    public Tilemap tilemap;
    //public List<*something*> skills;

}

public enum RaceType
{
    Aberration,
    Beast,
    Celestial,
    Construct,
    Elemental,
    Fey,
    Fiend,
    Human
}