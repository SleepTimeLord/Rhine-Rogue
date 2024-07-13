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
    public GameObject grid;
    public Dictionary<Vector2Int, TileProperties> nodeMap = new();
    public MouseControlActions mouseInput;
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
        mouseInput = new MouseControlActions();
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
            (Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> nodes, Vector2Int location) path = Entity.FindPathsAtPoint(entity.position, playerEntities, -1);
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        Ray ray = Camera.main.ScreenPointToRay(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        Debug.DrawRay(mousePosition, Camera.main.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2Int position = (Vector2Int) hit.collider.gameObject.GetComponent<TileProperties>().position;
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        Ray ray = Camera.main.ScreenPointToRay(mouseInput.MouseInput.MousePosition.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int cellPosition = hit.collider.gameObject.GetComponent<TileProperties>().position;
            cursor.transform.position = nodeMap[(Vector2Int)cellPosition].transform.position + new Vector3(0, 0.501f, 0);
        }
    }

    private void GenerateMap()
    {
        foreach(Transform child in grid.transform)
        {
            Vector3Int position = child.GetComponent<TileProperties>().position;
            if (!nodeMap.ContainsKey((Vector2Int) position))
            {
                nodeMap.Add((Vector2Int)position, child.GetComponent<TileProperties>());
            }
            else if (nodeMap[(Vector2Int)position].GetComponent<TileProperties>().position.y < position.y)
            {
                nodeMap[(Vector2Int)position] = child.GetComponent<TileProperties>();
            }
        }
        MapGenerated();
    }
}
