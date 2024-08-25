using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class FogTileManager : MonoBehaviour
{
    private Dictionary<GameObject, bool> fogTileStates = new Dictionary<GameObject, bool>();
    private List<Vector3> obstaclePositions = new List<Vector3>();
    private List<(Vector3 position, float radius)> lightSources = new List<(Vector3 position, float radius)>();
    public GameObject LightSources;
    public GameObject grid;
    public Dictionary<Vector3Int, TileProperties> ground = new Dictionary<Vector3Int, TileProperties>();
    //public HideEnemy hideEnemy;
    void Start()
    {
        if (CreateFog.fogInitialized)
        {
            InitializeFogTiles();
            InitializeObstacles();
            InitializeLightRadius();
        }
    }

    void InitializeFogTiles()
    {
        GameObject[] fogTiles = GameObject.FindGameObjectsWithTag("Fog");
        foreach (GameObject fogTile in fogTiles)
        {
            fogTileStates[fogTile] = true; // Initialize all fog tiles as active
            fogTile.SetActive(true); // Ensure they are visible initially
        }
    }

    void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
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
        lightSources.Add((position, radius));
        //print(lightSources.Count);
    }

    public void ClearLightSources()
    {
        lightSources.Clear();
    }

    public void InitializeMapUpdate() // updates map
    {
        // InitializeLightRadius is the catalyst for the lightsources
        InitializeLightRadius();
        if (lightSources.Count > 0)
        {
            ResetFogTileStates();
            foreach (var lightSource in lightSources)
            {
                UpdateFogTileState(lightSource.position, lightSource.radius);
            }
            ClearLightSources();
            InitializeFogTiles();
            InitializeGroundIsCoveredByFog();
        }
        else
        {
            Debug.Log("No light sources registered, skipping update");
        }
    }

    private void InitializeLightRadius() // gets the light sources in the gameobject and Startup them in the PlayerLightRadius script
    {
        foreach (Transform child in LightSources.transform)
        {
            PlayerLightRadius playerLight = child.GetComponent<PlayerLightRadius>();
            if (playerLight != null)
            {
                playerLight.StartupLightSource();
                //print("This is the light source name: " + child.name);
            }
            else
            {
                print(child.name + " doesn't have PlayerLightRadius script");
            }

        }
    }

    void UpdateFogTileState(Vector3 lightPosition, float lightRadius)
    {
        foreach (var fogTile in fogTileStates.Keys.ToList())
        {
            if (fogTile == null) continue;

            float distance = Vector3.Distance(lightPosition, fogTile.transform.position);
            if (distance <= lightRadius)
            {
                if (!IsFogTileBlocked(fogTile, lightPosition))
                {
                    fogTileStates[fogTile] = false;
                    fogTile.SetActive(false);
                }
            }
        }
    }

    bool IsFogTileBlocked(GameObject fogTile, Vector3 lightPosition)
    {
        Vector3 direction = (fogTile.transform.position - lightPosition).normalized;
        float distance = Vector3.Distance(lightPosition, fogTile.transform.position);

        foreach (Vector3 obstaclePosition in obstaclePositions)
        {
            float obstacleDistance = Vector3.Distance(lightPosition, obstaclePosition);
            if (obstacleDistance < distance)
            {
                Ray ray = new Ray(lightPosition, direction);
                if (Physics.Raycast(ray, out RaycastHit hit, distance, LayerMask.GetMask("Obstacle")))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.red);
                    return true;
                }
                else
                {
                    //Debug.DrawLine(ray.origin, ray.origin + direction * distance, Color.green);
                }
            }
        }
        return false;
    }

    public void InitializeGroundIsCoveredByFog()
    {
        CreateFog createFog = FindObjectOfType<CreateFog>();
        Dictionary<Vector3Int, GameObject> fogTiles = createFog.fogTilesByGroundPosition;

        foreach (Transform child in grid.transform)
        {
            TileProperties tileProperties = child.GetComponent<TileProperties>();
            Vector3Int groundPosition = tileProperties.position;

            if (!ground.ContainsKey(groundPosition))
            {
                ground.Add(groundPosition, tileProperties);
            }

            // Check if the ground position has a corresponding fog tile
            if (fogTiles.ContainsKey(groundPosition) && fogTiles[groundPosition].activeSelf)
            {
                tileProperties.isUnderFog = true;
            }
            else
            {
                tileProperties.isUnderFog = false;
            }
        }

        // Final log to verify results
        //foreach (var entry in ground)
        //{
        //    Debug.Log($"Position: {entry.Key}, Tile: {entry.Value}, IsUnderFog: {entry.Value.isUnderFog}");
        //}
    }

}
