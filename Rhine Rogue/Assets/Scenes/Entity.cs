using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public const float yOffset = 0.375f;
    public float health;
    public Vector3Int position;
    public RaceType raceType;
    public int speed;
    public int initiative;
    public int baseInitiative;
    public float attack;
    public Tilemap tilemap;
    public GameManager gameManager;
    //public List<*something*> skills;

    public virtual void MoveTo(Vector3Int position)
    {
        Vector3Int nodePosition = gameManager.nodeMap[(Vector2Int)position].GetComponent<TileProperties>().position;
        transform.position = tilemap.GetCellCenterWorld(nodePosition) + new Vector3(0, yOffset);
        this.position = nodePosition;
    }

    public Dictionary<Vector2Int, Vector2Int> FindPathsAtPoint(Vector2Int origin)
    {
        Dictionary<Vector2Int, Vector2Int> paths = new();
        Dictionary<Vector2Int, GameObject> map = gameManager.nodeMap;
        List<Vector2Int> nodes = new();
        Dictionary<Vector2Int, float> movementCost = new();
        nodes.Add(origin);
        movementCost[origin] = speed;
        
        while(nodes.Count > 0)
        {
            Vector2Int currentNode = nodes[0];
            Vector2Int neighbor = currentNode + new Vector2Int(1, 0);
            CheckTile(neighbor, nodes, paths, map, movementCost, currentNode);
            neighbor = currentNode + new Vector2Int(-1, 0);
            CheckTile(neighbor, nodes, paths, map, movementCost, currentNode);
            neighbor = currentNode + new Vector2Int(0, 1);
            CheckTile(neighbor, nodes, paths, map, movementCost, currentNode);
            neighbor = currentNode + new Vector2Int(0, -1);
            CheckTile(neighbor, nodes, paths, map, movementCost, currentNode);
            nodes.RemoveAt(0);
        }
        paths.Remove(origin);
        return paths;
    }

    private void CheckTile(Vector2Int neighbor, List<Vector2Int> nodes, Dictionary<Vector2Int, Vector2Int> paths, Dictionary<Vector2Int, GameObject> map, Dictionary<Vector2Int, float> movementCost, Vector2Int currentNode)
    {
        if (map.ContainsKey(neighbor) && CanMoveToTile(currentNode, neighbor) && movementCost[currentNode] - map[neighbor].GetComponent<TileProperties>().movementCost >= 0)
        {
            movementCost[neighbor] = movementCost[currentNode] - map[neighbor].GetComponent<TileProperties>().movementCost;
            paths[neighbor] = currentNode;

            int leftIndex = 0;
            int rightIndex = nodes.Count;
            while (rightIndex - leftIndex != 1) // Binary insertion sort
            {
                int index = leftIndex + (rightIndex - leftIndex) / 2;
                if (movementCost[nodes[index]] > movementCost[neighbor])
                {
                    leftIndex = index;
                }
                else if (movementCost[nodes[index]] < movementCost[neighbor])
                {
                    rightIndex = index;
                }
                else
                {
                    rightIndex = index;
                    leftIndex = rightIndex - 1;
                }
            }
            nodes.Insert(rightIndex, neighbor);
        }
    }

    protected bool CanMoveToTile(Vector2Int origin, Vector2Int neighbor)
    {
        TileProperties originNode = gameManager.nodeMap[origin].GetComponent<TileProperties>();
        TileProperties neighborNode = gameManager.nodeMap[neighbor].GetComponent<TileProperties>();
        return Mathf.Abs(originNode.position.z - neighborNode.position.z) <= 1 && neighborNode.terrain == TerrainType.Ground;
    }
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