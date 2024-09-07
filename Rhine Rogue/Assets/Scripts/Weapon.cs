using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public List<Vector2Int> weaponRange;
    public float damagePower;
    public int blastRadius;
    public float damageFalloff;
    public string weaponName;
    public List<StatusEffect> effects;

    public bool Attack(Vector2Int position, Entity user)
    {
        if (weaponRange.Contains(position - (Vector2Int)user.position))
        {
            if(user is PlayerEntity player)
            {
                player.RemainingActions--;
            }
            for(int i = 0; i <= blastRadius; i++)
            {
                float currentDamage;
                if (blastRadius != 0)
                    currentDamage = user.attack * damagePower * (damageFalloff + (1 - damageFalloff) * ((blastRadius - i) / blastRadius));
                else
                    currentDamage = user.attack * damagePower;
                print(currentDamage);
                for(int x = i; x >= -i; x--)
                {
                    int y = i - Mathf.Abs(x);
                    DoDamage(position + new Vector2Int(x, y), currentDamage);
                    if(y != 0)
                    {
                        DoDamage(position + new Vector2Int(x, -y), currentDamage);
                    }
                }
            }
            return true;
        }
        return false;
    }

    private void DoDamage(Vector2Int position, float damage)
    {
        var map = Entity.gameManager.nodeMap;
        if (map.ContainsKey(position) && map[position].occupier != null)
        {
            Entity occupier = map[position].occupier;
            print($"Did {damage} damage to {occupier.name}: {occupier.CurrentHealth}/{occupier.health}");
            map[position].occupier.CurrentHealth -= damage;
        }
    }
}