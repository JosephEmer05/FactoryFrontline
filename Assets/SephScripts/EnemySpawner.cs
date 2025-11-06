using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public int pathIndex;
    public bool isHighSpawner;

    public void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"Missing enemy prefab on spawner {gameObject.name}");
            return;
        }

        if (WaypointManager.Instance == null)
        {
            Debug.LogWarning("No WaypointManager instance found in scene!");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();

        if (enemy == null)
        {
            Debug.LogWarning($"Spawned enemy '{enemyObj.name}' does not have a BaseEnemy-derived script!");
            return;
        }

        if (isHighSpawner)
            enemy.AssignPath(WaypointManager.Instance.GetHighPathRandom());
        else
            enemy.AssignPath(WaypointManager.Instance.GetGroundPath(pathIndex));
    }
}
