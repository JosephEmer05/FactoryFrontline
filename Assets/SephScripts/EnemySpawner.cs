using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject lowEnemyPrefab;
    public GameObject highEnemyPrefab;

    [Header("Spawner Settings")]
    public int spawnIndex;
    public bool isHighSpawner;
    public float spawnInterval = 2f;

    public void SpawnEnemyFromWave()
    {
        GameObject enemyObj;

        if (isHighSpawner)
            enemyObj = Instantiate(highEnemyPrefab, transform.position, Quaternion.identity);
        else
            enemyObj = Instantiate(lowEnemyPrefab, transform.position, Quaternion.identity);

        EnemyBehavior enemy = enemyObj.GetComponent<EnemyBehavior>();

        if (isHighSpawner)
            enemy.AssignPath(WaypointManager.Instance.GetHighPathRandom());
        else
            enemy.AssignPath(WaypointManager.Instance.GetGroundPath(spawnIndex));
    }
}
