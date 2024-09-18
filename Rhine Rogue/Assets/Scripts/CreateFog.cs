using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFog : MonoBehaviour
{
    public GameObject grid;
    public GameObject fogPrefab;
    public static bool fogInitialized = false;

    // Dictionary to store fog tiles based on their corresponding ground tile positions
    public Dictionary<Vector3Int, GameObject> fogTilesByGroundPosition = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        InitializeFogGrid();
    }

    public void InitializeFogGrid()
    {

        foreach (Transform child in grid.transform)
        {
            Vector3 position = child.position;
            Vector3Int groundPosition = new Vector3Int(
                Mathf.FloorToInt(position.x),
                Mathf.FloorToInt(position.z),
                Mathf.FloorToInt(position.y)
            );

            // Place fog tile if conditions are met
            GameObject fogTile = CreateFogTileAboveBlock(child);
            if (fogTile != null)
            {
                print("Fog created above tile.");
                fogTilesByGroundPosition[groundPosition] = fogTile;
            }
        }

        fogInitialized = true;
    }

    GameObject CreateFogTileAboveBlock(Transform gridTile)
    {
        // Cast a ray from above the grid tile downwards to detect the top surface
        RaycastHit hit;
        Vector3 blockPosition = gridTile.position;
        Vector3 rayStartPos = new Vector3(blockPosition.x, blockPosition.y + 10, blockPosition.z);  // Start ray 10 units above the block

        // If there's no obstacle, proceed with placing the fog tile
        LayerMask gridLayer = gridTile.gameObject.layer;  // Get the layer of the grid tile

        if (Physics.Raycast(rayStartPos, Vector3.down, out hit, Mathf.Infinity, 1 << gridLayer))
        {
            // Place the fog tile slightly above the detected surface
            Vector3 fogPosition = new Vector3(hit.point.x, 3f,hit.point.z);
            GameObject fogTile = Instantiate(fogPrefab, fogPosition, Quaternion.identity);

            fogTile.transform.localScale = new Vector3(1, 5, 1);
            fogTile.layer = LayerMask.NameToLayer("Fog");
            fogTile.tag = "Fog";

            // Set the fog tile as a child of the grid tile it was created above
            fogTile.transform.SetParent(gridTile);

            // Perform a raycast downwards to detect if another fog tile exists above and destroy the current one if another fog tile is detected
            if (Physics.Raycast(fogPosition, Vector3.down, 2.0f, LayerMask.GetMask("Fog")))
            {
                Destroy(fogTile);
                return null; // Return null if the fog tile was destroyed
            }

            return fogTile; // Return the created fog tile
        }

        return null; // Return null if no fog tile was created
    }
}

