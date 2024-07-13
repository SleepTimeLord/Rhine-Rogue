using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScan : MonoBehaviour
{
    public GameObject grid; 
    public GameObject fogPrefab;
    public Transform fogContainer; 

    void Awake()
    {
        InitializeFogGrid();
    }

    void InitializeFogGrid()
    {
        if (fogContainer == null)
        {
            fogContainer = new GameObject("FogContainer").transform;
        }

        foreach (Transform child in grid.transform)
        {
            Vector3 position = child.position;
            CreateFogTileAboveBlock(position);
        }
    }

    void CreateFogTileAboveBlock(Vector3 blockPosition)
    {
        // Cast a ray from above the block downwards to detect the top surface
        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(blockPosition.x, blockPosition.y + 10, blockPosition.z);  // Start ray 10 units above the block

        // First, check if there's an obstacle below
        if (!Physics.Raycast(rayStartPos, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Obstacle")))
        {
            // If there's no obstacle, proceed with placing the fog tile
            if (Physics.Raycast(rayStartPos, Vector3.down, out hit))
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
                }
            }
        }
    }
}

