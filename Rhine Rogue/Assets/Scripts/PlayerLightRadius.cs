using UnityEngine;

public class PlayerLightRadius : MonoBehaviour
{
    public float lightRadius = 5f;
    private FogTileManager fogTileManager;
    public static bool lightInitialized = false;

    void Start()
    {
        fogTileManager = FindObjectOfType<FogTileManager>();
        StartupLightSource();
    }

    void Update()
    {
     
    }
    public void StartupLightSource()
    {
        if (fogTileManager != null && CreateFog.fogInitialized)
        {
            fogTileManager.RegisterLightSource(transform.position, lightRadius);
            lightInitialized = true;
            //print(gameObject.name);
        }
    }
}