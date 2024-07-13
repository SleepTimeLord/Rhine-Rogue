using UnityEngine;

public class PlayerLightRadius : MonoBehaviour
{
    public float lightRadius = 5f;
    private FogTileManager fogTileManager;

    void Start()
    {
        fogTileManager = FindObjectOfType<FogTileManager>();
    }

    void Update()
    {
        if (fogTileManager != null)
        {
            fogTileManager.RegisterLightSource(transform.position, lightRadius);
        }
    }
}