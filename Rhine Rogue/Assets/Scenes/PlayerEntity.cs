using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerEntity : Entity
{
    public static TextMeshProUGUI basicInfo;
    private Dictionary<Vector2Int, (Vector2Int previousNode, float cost)> possibleSquares = new();
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

    public override void Initialize()
    {
        currentHealth = health;
        possibleSquares = FindPathsAtPoint((Vector2Int)position, speed);
        base.Initialize();
    }

    public void UpdateBasicInfo()
    {
        basicInfo.text = $"HP: {currentHealth}/{health}\nEnergy: {remainingEnergy}/{speed}";
    }

    public bool MovePlayerTo(Vector3Int position)
    {
        if (possibleSquares.ContainsKey((Vector2Int)position) && gameManager.nodeMap[(Vector2Int)position].GetComponent<TileProperties>().occupier == null)
        {
            HideSquares();
            RemainingEnergy -= possibleSquares[(Vector2Int)position].cost;
            MoveTo(position);;
            RevealSquares();
            return true;
        }
        return false;
    }

    public override void MoveTo(Vector3Int position)
    {
        base.MoveTo(position);
        CalculateSquares();
    }

    public void CalculateSquares()
    {
        possibleSquares = FindPathsAtPoint((Vector2Int)this.position, remainingEnergy);
    }

    public void RevealSquares()
    {
        foreach(var key in possibleSquares.Keys)
        {
            if (gameManager.nodeMap[key].GetComponent<TileProperties>().occupier == null)
                gameManager.nodeMap[key].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void HideSquares()
    {
        foreach (var key in possibleSquares.Keys)
        {
            gameManager.nodeMap[key].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
