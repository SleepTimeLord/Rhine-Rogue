using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerEntity : Entity
{
    public static TextMeshProUGUI basicInfo;
    private Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> possibleSquares = new();
    public int lightRadius;
    private float remainingEnergy;
    public float RemainingEnergy
    {
        get => remainingEnergy;
        set 
        {
            remainingEnergy = value;
            UpdateBasicInfo();
        }
    }
    private int remainingActions;
    public int RemainingActions
    {
        get => remainingActions;
        set
        {
            remainingActions = value;
            UpdateBasicInfo();
        }
    }

    protected override void Start()
    {
        base.Start();
        remainingActions = actions;
    }

    public override void Initialize()
    {
        CurrentHealth = health;
        possibleSquares = FindPathsAtPoint((Vector2Int)position, speed);
        base.Initialize();
    }

    public void UpdateBasicInfo()
    {
        basicInfo.text = $"HP: {CurrentHealth}/{health}\nEnergy: {remainingEnergy}/{speed}\nActions: {remainingActions}/{actions}";
    }

    public bool MovePlayerTo(Vector2Int position)
    {
        if (possibleSquares.ContainsKey(position) && gameManager.nodeMap[position].occupier == null)
        {
            HideSquares();
            RemainingEnergy -= possibleSquares[position].cost;
            MoveTo(position);;
            RevealSquares();
            return true;
        }
        return false;
    }

    public override void MoveTo(Vector2Int position)
    {
        base.MoveTo(position);
        CalculateSquares();
    }

    public void CalculateSquares()
    {
        possibleSquares = FindPathsAtPoint(position, remainingEnergy);
    }

    public void RevealSquares()
    {
        foreach(var key in possibleSquares.Keys)
        {
            if (gameManager.nodeMap[key].occupier == null)
                gameManager.nodeMap[key].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }
    public void RevealSquares(Weapon weapon)
    {
        foreach (var key in weapon.weaponRange)
        {
            if (gameManager.nodeMap.ContainsKey(position + key))
                gameManager.nodeMap[position + key].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        }
    }

    public void HideSquares()
    {
        foreach (var key in possibleSquares.Keys)
        {
            gameManager.nodeMap[key].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    public void HideSquares(Weapon weapon)
    {
        foreach (var key in weapon.weaponRange)
        {
            if (gameManager.nodeMap.ContainsKey(position + key))
                gameManager.nodeMap[position + key].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
