using UnityEngine;

public class PlayerLightRadius : MonoBehaviour
{
    public float lightRadius = 5f;
    private FogTileManager fogTileManager;
    public static bool lightInitialized = false;

    void Start()
    {
        fogTileManager = FindObjectOfType<FogTileManager>();
        RegisterLightSource();
    }

    void Update()
    {
     
    }
    public void RegisterLightSource()
    {
        if (fogTileManager != null && MapScan.fogInitialized)
        {
            fogTileManager.RegisterLightSource(transform.position, lightRadius);
            lightInitialized = true;
            print(gameObject.name);
        }
    }
}