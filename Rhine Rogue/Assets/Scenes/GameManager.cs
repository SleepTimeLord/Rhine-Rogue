using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float initialTileYOffset = 0.375f;
    private const float tileHeightOffset = 0.095f;
    public Tilemap tilemap;
    public Dictionary<Vector2Int, TileProperties> nodeMap = new();
    public Mouse mouseInput;
    public GameObject overlayTile;
    public GameObject overlayContainer;
    public GameObject cursor;

    public GameObject weaponPanel;
    public GameObject weaponButton;

    public GameObject lightPrefab;
    private Weapon selectedWeapon;


    public List<Entity> playerEntities;
    public PlayerEntity currentPlayer;
    public GameObject entities;
    public List<Entity> initiativeList;
    public int turnIndex = 0;
    private int initializedEntites = 0;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI nextTurnText;
    public TextMeshProUGUI basicInfo;

    public delegate void MapGeneration();
    public static event MapGeneration MapGenerated;

    private void Awake()
    {
        mouseInput = new Mouse();
    }

    private void OnEnable()
    {
        mouseInput.Enable();
        Entity.EntityInitilized += RankInitiative;
    }

    private void OnDisable()
    {
        mouseInput.Disable();
        Entity.EntityInitilized -= RankInitiative;
    }

    private void SelectWeapon(Weapon weapon)
    {
        if (currentPlayer.RemainingActions != 0)
        {
            if (selectedWeapon != null)
            {
                currentPlayer.HideSquares(weapon);
            }
            else
            {
                currentPlayer.HideSquares();
            }
            selectedWeapon = weapon;
            currentPlayer.RevealSquares(weapon);
        }
    }

    private void RankInitiative()
    {
        initializedEntites++;
        if(initializedEntites == entities.transform.childCount)
        {
            foreach (Transform child in entities.transform)
            {
                int inititiative = child.GetComponent<Entity>().initiative;
                int leftIndex = 0;
                int rightIndex = initiativeList.Count;
                while (leftIndex != rightIndex) // Binary insertion sort
                {
                    int index = leftIndex + (rightIndex - leftIndex) / 2;
                    if (initiativeList[index].initiative > inititiative)
                    {
                        leftIndex = index + 1;
                    }
                    else if (initiativeList[index].initiative < inititiative)
                    {
                        rightIndex = index;
                    }
                    else
                    {
                        rightIndex = index;
                        leftIndex = index;
                    }
                }
                initiativeList.Insert(rightIndex, child.GetComponent<Entity>());
                if(child.GetComponent<Entity>() is PlayerEntity player)
                {
                    playerEntities.Add(player);
                    /*GameObject light = Instantiate(lightPrefab, player.transform);
                    light.GetComponent<Light2D>().SetShapePath(new Vector3[] { });*/
                }
            }
            turnIndex = initiativeList.Count - 1;
            NextTurn();
        }
    }

    public void RemoveEntity(Entity entity)
    {
        int index =  initiativeList.IndexOf(entity);
        if(index <= turnIndex)
        {
            turnIndex--;
        }
        initiativeList.Remove(entity);
        nodeMap[entity.position].occupier = null;
        Destroy(entity.gameObject);
        if(currentPlayer != null)
        {
            currentPlayer.CalculateSquares();
            currentPlayer.RevealSquares();
        }
    }

    public void NextTurn()
    {
        turnIndex++;
        nextTurnText.text = string.Format(" - {0}'s Turn Next", initiativeList[(turnIndex + 1) % initiativeList.Count].gameObject.name);
        if (turnIndex >= initiativeList.Count)
        {
            turnIndex = 0;
        }
        Entity entity = initiativeList[turnIndex];
        turnText.text = string.Format("{0}'s Turn", entity.gameObject.name);

        if(currentPlayer != null)
        {
            if (selectedWeapon != null)
            {
                currentPlayer.HideSquares(selectedWeapon);
                selectedWeapon = null;
            }
            else
                currentPlayer.HideSquares();
        }

        if(entity is PlayerEntity player)
        {
            player.RemainingEnergy = player.speed;
            player.RemainingActions = player.actions;
            player.CalculateSquares();
            player.RevealSquares();
            foreach(Transform child in weaponPanel.transform)
            {
                Destroy(child.gameObject);
            }
            foreach(Weapon weapon in player.weapons)
            {
                GameObject newButton = Instantiate(weaponButton, weaponPanel.transform);
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectWeapon(weapon));
                newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.weaponName;
            }
            currentPlayer = player;
        }
        else
        {
            currentPlayer = null;
            (Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> nodes, Vector2Int location) path = Entity.FindPathsAtPoint((Vector2Int)entity.position, playerEntities, -1);
            if(path.location != (Vector2Int)entity.position)
            {
                Vector2Int destination = path.location;
                while(path.nodes[destination].cost > entity.speed && path.nodes[destination].previousNode != destination)
                {
                    destination = path.nodes[destination].previousNode;
                }
                if(destination == path.location)
                {
                    destination = path.nodes[destination].previousNode;
                }
                entity.MoveTo(destination);
            }
            NextTurn();
        }
    }

    private void Start()
    {
        Entity.gameManager = this;
        PlayerEntity.basicInfo = basicInfo;
        mouseInput.MouseInput.LeftClick.performed += _ => MouseClick();
        GenerateMap();
        foreach(Transform child in entities.transform)
        {
            Vector2Int pos = child.GetComponent<Entity>().position;
            nodeMap[pos].occupier = child.GetComponent<Entity>();
        }
    }

    private void MouseClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        if(raycastHits.Length > 0)
        {
            Vector2Int position = (Vector2Int) raycastHits.OrderByDescending(i => i.collider.transform.position.z).First().collider.gameObject.GetComponent<TileProperties>().position;
            if (currentPlayer != null)
            {
                if (selectedWeapon == null)
                {
                    currentPlayer.MovePlayerTo(position);
                }
                else
                {
                    selectedWeapon.Attack(position, currentPlayer);
                    currentPlayer.HideSquares(selectedWeapon);
                    selectedWeapon = null;
                    currentPlayer.RevealSquares();
                }
            }
        }
    }

    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        if (raycastHits.Length > 0)
        {
            Vector3Int cellPosition = raycastHits.OrderByDescending(i => i.collider.transform.position.z).First().collider.gameObject.GetComponent<TileProperties>().position;
            cursor.transform.position = tilemap.CellToWorld(cellPosition) + new Vector3(0, initialTileYOffset - tileHeightOffset, 1);
        }
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
                            nodeMap[(Vector2Int)currentTile] = Instantiate(overlayTile, tilemap.CellToWorld(currentTile) + new Vector3(0, initialTileYOffset, 0.2f), Quaternion.identity, overlayContainer.transform).GetComponent<TileProperties>();
                            if (z != 0 || true) // Sometimes needed, sometimes not, idk
                                nodeMap[(Vector2Int)currentTile].transform.position -= new Vector3(0, tileHeightOffset);
                            nodeMap[(Vector2Int)currentTile].SetVars(currentTile, 1, (Tile)tile);
                            nodeMap[(Vector2Int)currentTile].gameObject.name = ((Vector2Int)currentTile).ToString();
                            //nodeMap[(Vector2Int)currentTile].GetComponent<SpriteRenderer>().sortingLayerName = "Overlay";
                            //print($"Added key ({x}, {y}) at height {z}");
                        }
                    }
                }
            }
        }
        MapGenerated();
    }
}
