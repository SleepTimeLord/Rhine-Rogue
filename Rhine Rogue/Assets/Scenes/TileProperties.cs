using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour
{
    private const int spriteNameOffset = 12;
    public Vector3Int position;
    public TerrainType terrain;
    public int movementCost;
    public Tile tile;
    public Entity occupier;

    public void SetVars(Vector3Int position, int movementCost, Tile tile)
    {
        this.position = position;
        this.movementCost = movementCost;
        this.tile = tile;

        int tileNum = int.Parse(tile.sprite.name[spriteNameOffset..]);
        if ((tileNum >= 0 && tileNum <= 40) || (tileNum >= 61 && tileNum <= 63) || (tileNum >= 68 && tileNum <= 70))
            terrain = TerrainType.Ground;
        else if (tileNum >= 86 && tileNum <= 114)
            terrain = TerrainType.Water;
    }

    public override string ToString()
    {
        return $"position: {position} movementCost: {movementCost} terrain: {terrain}";
    }
}

public enum TerrainType
{
    Water,
    Ground
}