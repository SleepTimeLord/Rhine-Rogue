using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogTileManager : MonoBehaviour
{
    private Dictionary<GameObject, bool> fogTileStates = new Dictionary<GameObject, bool>();
    private List<Vector3> obstaclePositions = new List<Vector3>();
    private List<(Vector3 position, float radius)> lightSources = new List<(Vector3 position, float radius)>();

    void Start()
    {
        InitializeFogTiles();
        InitializeObstacles();
    }

    void InitializeFogTiles()
    {
        GameObject[] fogTiles = GameObject.FindGameObjectsWithTag("Fog");
        foreach (GameObject fogTile in fogTiles)
        {
            fogTileStates[fogTile] = true; // Initialize all fog tiles as active
            fogTile.SetActive(true); // Ensure they are visible initially
        }
        Debug.Log($"Found {fogTileStates.Count} fog tiles.");
    }

    void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            obstaclePositions.Add(obstacle.transform.position);
        }
        Debug.Log($"Found {obstaclePositions.Count} obstacles.");
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
    }

    public void ClearLightSources()
    {
        lightSources.Clear();
    }

    void Update()
    {
        if (lightSources.Count > 0)
        {
            ResetFogTileStates();
            foreach (var lightSource in lightSources)
            {
                UpdateFogTileState(lightSource.position, lightSource.radius);
            }
            ClearLightSources();
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
                    Debug.DrawLine(ray.origin, hit.point, Color.red);
                    return true;
                }
                else
                {
                    Debug.DrawLine(ray.origin, ray.origin + direction * distance, Color.green);
                }
            }
        }
        return false;
    }
}