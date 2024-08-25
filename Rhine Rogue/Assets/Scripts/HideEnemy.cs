using UnityEngine;
using UnityEngine.UIElements;

public class HideEnemy : MonoBehaviour
{
    private Renderer enemyRenderer;
    public FogTileManager groundState;
    private Vector2Int position;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        SetVisibility(true);
    }
    private void Update()
    {
        EnemyOnFog();
    }
    public void SetVisibility(bool isVisible)
    {
        // Hide/show enemy components like meshes, renderers, etc.
        enemyRenderer.enabled = isVisible;
    }

    public void EnemyOnFog()
    {
        Vector3Int position = new Vector3Int(this.position.x, 0, this.position.y);
        if (groundState.ground.TryGetValue(position, out TileProperties tileProperties) &&
            tileProperties.isUnderFog)
        {
            SetVisibility(false);
        }
        else
        {
            SetVisibility(true);
        }
    }
}