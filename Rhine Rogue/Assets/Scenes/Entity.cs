using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health;
    public RaceType raceType;
    public int speed;
    public int initiative;
    public float attack;
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