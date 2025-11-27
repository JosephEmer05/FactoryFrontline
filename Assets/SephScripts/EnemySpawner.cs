using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject lowEnemyPrefab;
    public GameObject highEnemyPrefab;

    [Header("Spawner Settings")]
    public int pathIndex;
    public bool isHighSpawner;
    public float spawnInterval = 2f;

    // Now returns the created enemy GameObject so callers can register it
    public GameObject SpawnEnemy(GameObject prefab = null)
    {
        if (WaypointManager.Instance == null)
        {
            Debug.LogWarning("No WaypointManager instance found in scene!");
            return null;
        }

        GameObject enemyObj;

        // If a prefab was explicitly passed use it; otherwise pick based on isHighSpawner
        if (prefab != null)
        {
            enemyObj = Instantiate(prefab, transform.position, Quaternion.identity);
        }
        else
        {
            if (isHighSpawner)
            {
                if (highEnemyPrefab == null)
                {
                    Debug.LogWarning("High enemy prefab is null on spawner.");
                    return null;
                }
                enemyObj = Instantiate(highEnemyPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                if (lowEnemyPrefab == null)
                {
                    Debug.LogWarning("Low enemy prefab is null on spawner.");
                    return null;
                }
                enemyObj = Instantiate(lowEnemyPrefab, transform.position, Quaternion.identity);
            }
        }

        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
        if (enemy == null)
        {
            Debug.LogWarning($"Spawned enemy '{enemyObj.name}' does not have a BaseEnemy-derived script!");
            return enemyObj;
        }

        // Assign path depending on spawner type
        if (isHighSpawner)
        {
            Transform[] path = WaypointManager.Instance.GetHighPathRandom();
            enemy.AssignPath(path);
        }
        else
        {
            Transform[] path = WaypointManager.Instance.GetGroundPath(pathIndex);
            enemy.AssignPath(path);
        }

        return enemyObj;
    }
}
