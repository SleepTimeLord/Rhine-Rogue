using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogTileManager : MonoBehaviour
{
    private List<Vector3> obstaclePositions = new List<Vector3>();
    private List<GameObject> obstaclesBlocks = new List<GameObject>();
    private List<TileProperties> blockProperties = new List<TileProperties>();
    private List<(Vector3 position, float radius)> lightSourcesInfo = new List<(Vector3 position, float radius)>();
    public GameObject grid;
    private Dictionary<GameObject, Material> originalMesh = new Dictionary<GameObject, Material>();
    public Material darkness;
    public Material obstaclecolor;

    void Start()
    {
        IntitializeTilePropertiesBlocks();
        InitializeObstacles();
        InitializeLightRadius();
        SaveMeshMaterial();
        
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
        foreach (TileProperties blockProperty in blockProperties)
        {
            // Set all tiles to be under fog by default
            GameObject obj = blockProperty.gameObject;
            Renderer renderer = obj.GetComponent<Renderer>();
            blockProperty.isUnderFog = true;

            if (blockProperty.position.z >= 1)
            {
                renderer.enabled = false;
            }
        }
    }

    void IntitializeTilePropertiesBlocks()
    {
        blockProperties = FindObjectsOfType<TileProperties>().ToList();
    }


    public void RegisterLightSource(Vector3 position, float radius)
    {
        lightSourcesInfo.Add((position, radius));
    }

    public void ClearLightSources()
    {
        lightSourcesInfo.Clear();
    }

    public void InitializeMapUpdate()
    {
        InitializeLightRadius();
        if (lightSourcesInfo.Count > 0)
        {
            ResetFogTileStates();
            foreach (var lightSource in lightSourcesInfo)
            {
                UpdateFogTileState(lightSource.position, lightSource.radius);
            }
            TurnToDarkness();
            CheckEnemyOnFog();
            ClearLightSources();
        }
    }


    private void InitializeLightRadius()
    {
        PlayerLightRadius[] lightSources = FindObjectsOfType<PlayerLightRadius>();

        lightSourcesInfo.Clear();

        foreach (PlayerLightRadius playerLight in lightSources)
        {
            if (playerLight != null)
            {
                playerLight.StartupLightSource();
                RegisterLightSource(playerLight.transform.position, playerLight.lightRadius);
            }
        }
    }


    void UpdateFogTileState(Vector3 lightPosition, float lightRadius)
    {
        foreach (TileProperties blockProperty in blockProperties)
        {
            GameObject obj = blockProperty.gameObject;
            Renderer renderer = obj.GetComponent<Renderer>();
            Vector3 gridTilePosition = blockProperty.transform.position + Vector3.up;
            float distance = Vector3.Distance(lightPosition, gridTilePosition);


            bool isObstacle = obj.CompareTag("Obstacle");
            // Check if the tile is within the current light radius and not blocked
            if (distance <= lightRadius)
            {
                if (isObstacle)
                {
                    blockProperty.isUnderFog = false;

                    if (blockProperty.position.z >= 1)
                    {
                        renderer.enabled = true;
                    }
                }
                else if (!IsFogTileBlocked(gridTilePosition, lightPosition))
                {
                    blockProperty.isUnderFog = false;

                    if (blockProperty.position.z >= 1)
                    {
                        renderer.enabled = true;
                    }
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
                    return true;
                }
            }
        }

        return false;
    }

    void SaveMeshMaterial()
    {
        TileProperties[] tileObjects = FindObjectsOfType<TileProperties>();

        foreach (TileProperties tileProperty in tileObjects)
        {
            GameObject obj = tileProperty.gameObject;
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

            if (renderer != null && !originalMesh.ContainsKey(obj))
            {
                originalMesh[obj] = renderer.material; // Save the original material
            }
        }
    }


    void TurnToDarkness()
    {

        foreach (TileProperties blockProperty in blockProperties)
        {
            GameObject obj = blockProperty.gameObject;
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

            if (renderer == null) continue;

            if (blockProperty.isUnderFog)
            {
                renderer.material = darkness; // Apply darkness material to fogged objects
            }
            else
            {
                if (originalMesh.TryGetValue(obj, out Material originalMaterial))
                {
                    renderer.material = originalMaterial; // Restore original material for visible objects
                }
            }
        }
    }



    public void CheckEnemyOnFog()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            Entity entity = enemy.GetComponent<Entity>();

            if (entity == null) continue;

            bool isVisible = false;
            foreach (TileProperties blockProperty in blockProperties)
            {
                if (blockProperty.occupier == entity)
                {
                    isVisible = !blockProperty.isUnderFog;
                    break;
                }
            }
            enemyRenderer.enabled = isVisible;
        }
    }
}
