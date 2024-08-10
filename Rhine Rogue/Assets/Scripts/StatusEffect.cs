using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public string effectName;
    public Entity entity;
    public abstract void StartEffect();
    public abstract void UpdateEffect();
    public abstract void EndEffect();
}
