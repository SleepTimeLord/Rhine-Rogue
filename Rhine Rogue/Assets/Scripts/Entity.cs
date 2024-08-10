using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public static GameManager gameManager;

    public const float yOffset = 0.501f;
    public float health;
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            if(value <= 0)
            {
                gameManager.RemoveEntity(this);
            }
            else
            {
                currentHealth = value;
            }
        }
    }    
    public Vector2Int position;
    public RaceType raceType;
    public int speed;
    public int initiative;
    public int baseInitiative;
    public float attack;
    public int actions;
    
    public Weapon[] weapons;

    public delegate void EntityInitilization();
    public static event EntityInitilization EntityInitilized;

    protected virtual void Start()
    {
        currentHealth = health;
    }

    void OnEnable()
    {
        GameManager.MapGenerated += Initialize;
    }

    void OnDisable()
    {
        GameManager.MapGenerated -= Initialize;
    }

    public virtual void Initialize()
    {
        MoveTo(position);
        RollInitiative();
        EntityInitilized();
    }

    public void RollInitiative()
    {
        initiative = baseInitiative + Random.Range(1, 21);
    }

    public virtual void MoveTo(Vector2Int position)
    {
        gameManager.nodeMap[this.position].occupier = null;
        Vector3Int nodePosition = gameManager.nodeMap[position].position; // Just in case the Z on position is wrong
        transform.position = gameManager.nodeMap[position].transform.position + new Vector3(0, yOffset);
        this.position = (Vector2Int) nodePosition;
        gameManager.nodeMap[position].occupier = this;
    }

    public static Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> FindPathsAtPoint(Vector2Int origin, float maxSearchDistance)
    {
        return FindPathsAtPoint(origin, null, maxSearchDistance).nodes;
    }

    /// <summary>
    /// Returns the location of the nearest target, the cost to go to the target, and the path towards it. Returns the origin if the target is not found.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targets"></param>
    /// <param name="maxSearchDistance"></param>
    /// <returns>The paths, movement costs to those nodes, and the node of a target</returns>
    public static (Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> nodes, Vector2Int location) FindPathsAtPoint(Vector2Int origin, List<Entity> targets, float maxSearchDistance)
    {
        Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> nodes = new();
        Dictionary<Vector2Int, TileProperties> map = gameManager.nodeMap;
        List<Vector2Int> nodeList = new();
        nodeList.Add(origin);
        nodes.Add(origin, (origin, 0));
        
        while(nodeList.Count > 0)
        {
            Vector2Int currentNode = nodeList[0];
            Vector2Int neighbor = currentNode + new Vector2Int(1, 0);
            if(CheckTile(neighbor, nodes, nodeList, map, currentNode, maxSearchDistance, targets))
            {
                return (nodes, neighbor);
            }
            neighbor = currentNode + new Vector2Int(-1, 0);
            if(CheckTile(neighbor, nodes, nodeList, map, currentNode, maxSearchDistance, targets))
            {
                return (nodes, neighbor);
            }
            neighbor = currentNode + new Vector2Int(0, 1);
            if(CheckTile(neighbor, nodes, nodeList, map, currentNode, maxSearchDistance, targets))
            {
                return (nodes, neighbor);
            }
            neighbor = currentNode + new Vector2Int(0, -1);
            if(CheckTile(neighbor, nodes, nodeList, map, currentNode, maxSearchDistance, targets))
            {
                return (nodes, neighbor);
            }
            nodeList.RemoveAt(0);
        }
        return (nodes, origin);
    }

    private static bool CheckTile(Vector2Int neighbor, Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> nodes, List<Vector2Int> nodeList, Dictionary<Vector2Int, TileProperties> map, Vector2Int currentNode, float maxSearchDistance, List<Entity> targets)
    {
        if (map.ContainsKey(neighbor) && CanMoveToTile(currentNode, neighbor) && (nodes[currentNode].cost + map[neighbor].movementCost <= maxSearchDistance || maxSearchDistance == -1) && (map[neighbor].occupier is PlayerEntity || map[neighbor].occupier == null))
        {
            float costToMove = nodes[currentNode].cost + map[neighbor].movementCost;
            if (!nodes.ContainsKey(neighbor) || nodes[neighbor].cost > costToMove)
            {
                nodes[neighbor] = (currentNode, costToMove);
                if(targets != null && targets.Contains(map[neighbor].occupier))
                {
                    return true;
                }
                int leftIndex = 0;
                int rightIndex = nodeList.Count;
                
                while (leftIndex != rightIndex) // Binary insertion sort
                {
                    int index = leftIndex + (rightIndex - leftIndex) / 2;
                    if (nodes[nodeList[index]].cost < nodes[neighbor].cost)
                    {
                        leftIndex = index + 1;
                    }
                    else if (nodes[nodeList[index]].cost > nodes[neighbor].cost)
                    {
                        rightIndex = index;
                    }
                    else
                    {
                        rightIndex = index;
                        leftIndex = index;
                    }
                }
                nodeList.Insert(rightIndex, neighbor);
            }
        }
        return false;
    }

    protected static bool CanMoveToTile(Vector2Int origin, Vector2Int neighbor)
    {
        TileProperties originNode = gameManager.nodeMap[origin];
        TileProperties neighborNode = gameManager.nodeMap[neighbor];
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