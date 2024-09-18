using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FogTileManager : MonoBehaviour
{
    private Dictionary<GameObject, bool> fogTileStates = new Dictionary<GameObject, bool>();
    private List<Vector3> obstaclePositions = new List<Vector3>();
    private List<GameObject> obstaclesBlocks = new List<GameObject>();
    private List<(Vector3 position, float radius)> lightSourcesInfo = new List<(Vector3 position, float radius)>();
    public GameObject grid;
    //public Dictionary<Vector3Int, TileProperties> ground = new Dictionary<Vector3Int, TileProperties>();

    void Start()
    {
        InitializeFogTiles();
        InitializeObstacles();
        InitializeLightRadius();
    }

    void InitializeFogTiles()
    {
        GameObject[] fogTiles = GameObject.FindGameObjectsWithTag("Fog");
        foreach (GameObject fogTile in fogTiles)
        {
            Renderer renderer = fogTile.GetComponent<Renderer>();
            fogTileStates[fogTile] = true; // Initialize all fog tiles as active
            renderer.enabled = true;
            fogTile.SetActive(true); // Ensure they are visible initially
        }
    }

    void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            obstaclesBlocks.Add(obstacle);
            obstaclePositions.Add(obstacle.transform.position);
        }
    }

    public void ResetFogTileStates()
    {
        foreach (var fogTile in fogTileStates.Keys.ToList())
        {
            if (fogTile == null)
            {
                fogTileStates.Remove(fogTile);
                continue;
            }

            fogTileStates[fogTile] = true;
            fogTile.SetActive(true); // Ensure all fog tiles are active
        }
    }

    public void RegisterLightSource(Vector3 position, float radius)
    {
        lightSourcesInfo.Add((position, radius));
    }

    public void ClearLightSources()
    {
        lightSourcesInfo.Clear();
    }

    public void InitializeMapUpdate() // updates map
    {
        // InitializeLightRadius is the catalyst for the light sources
        InitializeLightRadius();
        if (lightSourcesInfo.Count > 0)
        {
            ResetFogTileStates();
            foreach (var lightSource in lightSourcesInfo)
            {
                UpdateFogTileState(lightSource.position, lightSource.radius);
                //UpdateObstacleVisiblity(lightSource.position, lightSource.radius);
            }
            ClearLightSources();
            InitializeFogTiles();
            InitializeGroundIsCoveredByFog();
            CheckEnemyOnFog();
        }
        else
        {
            Debug.Log("No light sources registered, skipping update");
        }
    }

    private void InitializeLightRadius() // gets all light sources with the PlayerLightRadius script and initializes them
    {
        PlayerLightRadius[] lightSources = FindObjectsOfType<PlayerLightRadius>();
        foreach (PlayerLightRadius playerLight in lightSources)
        {
            if (playerLight != null)
            {
                playerLight.StartupLightSource();
                // Optional: print("Light source initialized: " + playerLight.gameObject.name);
            }
        }
    }

    //private void UpdateObstacleVisiblity(Vector3 lightPosition, float lightRadius)
    //{
    //    foreach (GameObject obstacleBlock in obstaclesBlocks)
    //    {
    //        if (obstacleBlock == null) continue;

    //        Renderer obstacleRenderer = obstacleBlock.GetComponent<Renderer>();

    //        bool isVisible = false;
    //        //obstacleRenderer.enabled = false;
    //        foreach (var fogTile in fogTileStates.Keys.ToList())
    //        {
    //            if (fogTile == null) continue;

    //            float distance = Vector3.Distance(lightPosition, fogTile.transform.position);
    //            //print(distance);
    //            //print(lightRadius);
    //            if(distance <= lightRadius)
    //            {
    //                print(obstacleBlock.name + " is in light");
    //                isVisible = true;
    //                //obstacleRenderer.enabled = true;
    //                break;
    //            }
    //            else
    //            {
    //                print(obstacleBlock.name + " is not in light LOOK LOOK");
    //            }
    //        }
    //        obstacleRenderer.enabled = isVisible;

    //    }
    //}

    void UpdateFogTileState(Vector3 lightPosition, float lightRadius)
    {
        foreach (var fogTile in fogTileStates.Keys.ToList())
        {
            if (fogTile == null) continue;

            // Use the parent grid tile's position plus Vector3.up to simulate the fog tile's position
            Vector3 gridTilePosition = fogTile.transform.parent.position;
            Vector3 fogPosition = gridTilePosition + Vector3.up;

            float distance = Vector3.Distance(lightPosition, fogPosition);
            if (distance <= lightRadius)
            {
                // Check if the fog tile is blocked by obstacles or part of the obstacles list
                if (!IsFogTileBlocked(fogPosition, lightPosition))
                {
                    fogTileStates[fogTile] = false;
                    fogTile.SetActive(false);  // Disable the fog tile if within the light radius
                }
                else if (obstaclePositions.Contains(gridTilePosition))
                {
                    print(obstaclePositions);
                    fogTileStates[fogTile] = true;
                    fogTile.SetActive(true);
                }
            }
        }
    }


    bool IsFogTileBlocked(Vector3 fogPosition, Vector3 lightPosition)
    {
        Vector3 direction = (fogPosition - lightPosition).normalized;
        float distance = Vector3.Distance(lightPosition, fogPosition);

        foreach (Vector3 obstaclePosition in obstaclePositions)
        {
            float obstacleDistance = Vector3.Distance(lightPosition, obstaclePosition);
            if (obstacleDistance < distance)
            {
                Ray ray = new Ray(lightPosition, direction);
                if (Physics.Raycast(ray, out RaycastHit hit, distance, LayerMask.GetMask("Obstacle")))
                {
                    return true; // The fog tile is blocked by an obstacle
                }
            }
        }
        return false; // The fog tile is not blocked
    }


    public void InitializeGroundIsCoveredByFog()
    {
        foreach (Transform child in grid.transform)
        {
            TileProperties tileProperties = child.GetComponent<TileProperties>();
            if (tileProperties == null) continue;

            // Check if any fog object is a child of the current tile
            bool isUnderFog = false;
            foreach (Transform fog in child)
            {
                GameObject fogObject = fog.gameObject;
                if (fogTileStates.TryGetValue(fogObject, out bool fogActive))
                {
                    // Set isUnderFog based on the active state of the fog tile
                    if (fogActive)
                    {
                        isUnderFog = true;
                        break; // Exit early since any active fog means the tile is under fog
                    }
                }
            }

            // Update the tile's fog state
            tileProperties.isUnderFog = isUnderFog;
        }
    }

    public void CheckEnemyOnFog()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        TileProperties[] tileProperties = FindObjectsOfType<TileProperties>();

        foreach (GameObject enemy in Enemies)
        {
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            Entity entity = enemy.GetComponent<Entity>();

            if (entity == null) continue;

            bool isVisible = false;
            foreach (TileProperties tileProperty in tileProperties)
            {
                if (tileProperty.occupier == null) continue;

                // match the enemy with the tile that it's on
                if (tileProperty.occupier == entity)
                {
                    isVisible = !tileProperty.isUnderFog;
                    break;
                }
            }
            enemyRenderer.enabled = isVisible;
        }

    }

}
