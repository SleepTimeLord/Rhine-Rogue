using System.Collections.Generic;
using UnityEngine;

public class CreateFog : MonoBehaviour
{
    public GameObject grid;
    public GameObject fogPrefab;
    public Transform fogContainer;
    public static bool fogInitialized = false;

    // Dictionary to store fog tiles based on their corresponding ground tile positions
    public Dictionary<Vector3Int, GameObject> fogTilesByGroundPosition = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        InitializeFogGrid();
    }

    public void InitializeFogGrid()
    {
        if (fogContainer == null)
        {
            fogContainer = new GameObject("FogContainer").transform;
        }

        foreach (Transform child in grid.transform)
        {
            Vector3 position = child.position;
            Vector3Int groundPosition = new Vector3Int(
                Mathf.FloorToInt(position.x),
                Mathf.FloorToInt(position.z),
                Mathf.FloorToInt(position.y)
            );

            // Place fog tile if conditions are met
            GameObject fogTile = CreateFogTileAboveBlock(position);
            if (fogTile != null)
            {
                fogTilesByGroundPosition[groundPosition] = fogTile;
            }
        }

        fogInitialized = true;
    }


    GameObject CreateFogTileAboveBlock(Vector3 blockPosition)
    {
        // Cast a ray from above the block downwards to detect the top surface
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(blockPosition.x, blockPosition.y + 10, blockPosition.z);  // Start ray 10 units above the block

        // First, check if there's an obstacle below
        if (!Physics.Raycast(rayStartPos, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Obstacle")))
        {
            // If there's no obstacle, proceed with placing the fog tile
            if (Physics.Raycast(rayStartPos, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Place the fog tile slightly above the detected surface
                Vector3 fogPosition = hit.point + Vector3.up * 0.5f;
                GameObject fogTile = Instantiate(fogPrefab, fogPosition, Quaternion.identity, fogContainer);
                fogTile.transform.localScale = new Vector3(1, 1, 1);
                fogTile.layer = LayerMask.NameToLayer("Fog");
                fogTile.tag = "Fog";

                // Perform a raycast downwards to detect if another fog tile exists above and if another fog tile is detected, destroy the current one
                if (Physics.Raycast(fogPosition, Vector3.down, 2.0f, LayerMask.GetMask("Fog")))
                {
                    Destroy(fogTile);
                    return null; // Return null if the fog tile was destroyed
                }

                return fogTile; // Return the created fog tile
            }
        }

        return null; // Return null if no fog tile was created
    }
}


