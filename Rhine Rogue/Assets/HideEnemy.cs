using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Renderer enemyRenderer;

    private void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
    }

    public void SetVisibility(bool isVisible)
    {
        // Hide/show enemy components like meshes, renderers, etc.
        enemyRenderer.enabled = isVisible;
    }
}



