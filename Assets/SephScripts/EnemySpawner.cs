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

    public void SpawnEnemyFromWave()
    {
        if (WaypointManager.Instance == null)
        {
            Debug.LogWarning("No WaypointManager instance found in scene!");
            return;
        }

        GameObject enemyObj;

        if (isHighSpawner)
            enemyObj = Instantiate(highEnemyPrefab, transform.position, Quaternion.identity);
        else
            enemyObj = Instantiate(lowEnemyPrefab, transform.position, Quaternion.identity);

        EnemyBehavior enemy = enemyObj.GetComponent<EnemyBehavior>();
        if (enemy == null)
        {
            Debug.LogWarning("Spawned enemy has no EnemyBehavior script!");
            return;
        }

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
    }
}
