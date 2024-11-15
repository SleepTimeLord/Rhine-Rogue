using System.Collections;
using UnityEngine;

public class UpdateGame : MonoBehaviour
{
    public FogTileManager updateMapState;

    private void Start()
    {
        UpdateMap(); // This will call InitializeMapUpdate() once at start
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdateMap();
        }
    }

    public void UpdateMap()
    {
        updateMapState.InitializeMapUpdate();
    }
}

