using System.Collections;
using UnityEngine;

public class FogTiles : MonoBehaviour
{
    private bool startEnemyScript = false;

    private void Start()
    {
        StartCoroutine(WaitBeforeStarting());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && startEnemyScript)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy hidden");
                enemy.SetVisibility(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && startEnemyScript)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy still in fog");
                enemy.SetVisibility(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && startEnemyScript)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetVisibility(true);
            }
        }
    }

    private IEnumerator WaitBeforeStarting()
    {
        yield return new WaitForSeconds(0.2f);
        startEnemyScript = true;
    }
}

