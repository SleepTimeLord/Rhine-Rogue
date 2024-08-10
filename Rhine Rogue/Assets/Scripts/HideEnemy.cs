using UnityEngine;

public class HideEnemy : MonoBehaviour
{
    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        SetVisibility(true);
    }

    public void SetVisibility(bool isVisible)
    {
        // Hide/show enemy components like meshes, renderers, etc.
        enemyRenderer.enabled = isVisible;
    }
}



