using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private const float initialTileYOffset = 0.375f;
    private const float tileHeightOffset = 0.095f;
    public Tilemap tilemap;
    public Dictionary<Vector2Int, GameObject> nodeMap = new();
    public Mouse mouseInput;
    public PlayerEntity playerEntity;
    public GameObject overlayTile;
    public GameObject overlayContainer;

    private void Awake()
    {
        mouseInput = new Mouse();
    }

    private void OnEnable()
    {
        mouseInput.Enable();
    }

    private void OnDisable()
    {
        mouseInput.Disable();
    }

    private void Start()
    {
        mouseInput.MouseInput.LeftClick.performed += _ => MouseClick();
        GenerateMap();
    }

    private void MouseClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        if(raycastHits.Length > 0)
        {
            Vector3Int position = raycastHits.OrderByDescending(i => i.collider.transform.position.z).First().collider.gameObject.GetComponent<TileProperties>().position;
            playerEntity.MovePlayerTo(position);
        }
    }

    private void Update()
    {
        
    }

    private void GenerateMap()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        for (int z = bounds.max.z - 1; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int currentTile = new Vector3Int(x, y, z);
                    if (!nodeMap.ContainsKey((Vector2Int) currentTile))
                    {
                        TileBase tile = tilemap.GetTile(currentTile);
                        if (tile != null)
                        {
                            nodeMap[(Vector2Int)currentTile] = Instantiate(overlayTile, tilemap.CellToWorld(currentTile) + new Vector3(0, initialTileYOffset, 1), Quaternion.identity, overlayContainer.transform);
                            if (z != 0)
                                nodeMap[(Vector2Int)currentTile].transform.position -= new Vector3(0, tileHeightOffset);
                            nodeMap[(Vector2Int)currentTile].GetComponent<TileProperties>().SetVars(currentTile, 1, (Tile)tile);
                            nodeMap[(Vector2Int)currentTile].GetComponent<SpriteRenderer>().sortingLayerName = "Overlay";
                            //print($"Added key ({x}, {y}) at height {z}");
                        }
                    }
                }
            }
        }
        playerEntity.Initialize();
    }
}
