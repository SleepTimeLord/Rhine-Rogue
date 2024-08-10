using System.Collections;
using UnityEngine;

public class UpdateFog : MonoBehaviour
{
    public FogTileManager UpdateMapState;
    private PlayerLightRadius PlayerLightRadius;
    public bool MapUpdating = false;

    private void Start()
    {
        PlayerLightRadius = FindAnyObjectByType<PlayerLightRadius>();
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
        StartCoroutine(UpdateDone());
        if (MapUpdating) {
            PlayerLightRadius.RegisterLightSource();
            UpdateMapState.InitializeMapUpdate();
        }
        else
        {
            print(MapUpdating);
        }

    }
    private IEnumerator UpdateDone()
    {
        MapUpdating = true;
        yield return new WaitForSeconds(0.1f);
        MapUpdating = false;
    }
}
