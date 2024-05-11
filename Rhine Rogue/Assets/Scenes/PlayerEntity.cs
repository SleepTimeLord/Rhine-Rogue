using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerEntity : Entity
{
    public Dictionary<Vector2Int, Vector2Int> possibleSquares = new();

    private void Start()
    {
        possibleSquares = FindPathsAtPoint((Vector2Int)position);
        MoveTo(position);
        print("Revealing");
        RevealSquares();
    }

    public void MovePlayerTo(Vector3Int position)
    {
        if (possibleSquares.ContainsKey((Vector2Int)position))
        {
            MoveTo(position);
        }
    }

    public override void MoveTo(Vector3Int position)
    {
        HideSquares();
        base.MoveTo(position);
        possibleSquares = FindPathsAtPoint((Vector2Int)this.position);
        RevealSquares();
    }

    private void RevealSquares()
    {
        foreach(var key in possibleSquares.Keys)
        {
            print("Done");
            gameManager.nodeMap[key].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    private void HideSquares()
    {
        foreach (var key in possibleSquares.Keys)
        {
            gameManager.nodeMap[key].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
