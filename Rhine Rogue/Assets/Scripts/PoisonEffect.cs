using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : StatusEffect
{
    public new string effectName = "Poison";
    public int damage;
    public override void StartEffect()
    {
        throw new System.NotImplementedException();
    }
    public override void UpdateEffect()
    {
        entity.CurrentHealth -= damage;
    }
    public override void EndEffect()
    {
        throw new System.NotImplementedException();
    }
}
