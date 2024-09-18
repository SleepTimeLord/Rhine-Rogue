using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour
{
    public Vector3Int position;
    public TerrainType terrain;
    public int movementCost;
    public Entity occupier;
    public bool isUnderFog;

    private void Start()
    {
        Vector3 position = transform.position;
        this.position = new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z), Mathf.FloorToInt(position.y));
        isUnderFog = false;
    }

    public override string ToString()
    {
        return $"position: {position} movementCost: {movementCost} terrain: {terrain}";
    }
}

public enum TerrainType
{
    Water,
    Ground,
    Obstacle
}