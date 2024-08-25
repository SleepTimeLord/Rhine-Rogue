using System.Collections;
using UnityEngine;

public class UpdateFog : MonoBehaviour
{
    public FogTileManager updateMapState;
    //public bool MapUpdating = false; //keeping this if needed
    //public GameObject LightSources;

    private void Start()
    {
        UpdateMap();
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

