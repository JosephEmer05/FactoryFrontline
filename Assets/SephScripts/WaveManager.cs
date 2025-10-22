using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner References")]
    public EnemySpawner[] lowSpawners;
    public EnemySpawner[] highSpawners;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 10f;
    public int enemiesPerWave = 5;
    public float spawnDelay = 0.5f;

    [Header("Difficulty Scaling")]
    public float enemyHealthMultiplier = 1.1f;
    public float enemyDamageMultiplier = 1.05f;
    public float spawnCountMultiplier = 1.15f;

    private int currentWave = 0;
    private bool isWaveActive = false;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            currentWave++;
            Debug.Log("Starting Wave " + currentWave);
            isWaveActive = true;

            yield return StartCoroutine(SpawnWaveGroup(lowSpawners));
            yield return StartCoroutine(SpawnWaveGroup(highSpawners));

            isWaveActive = false;

            enemiesPerWave = Mathf.CeilToInt(enemiesPerWave * spawnCountMultiplier);
            spawnDelay = Mathf.Max(0.2f, spawnDelay * 0.95f);

            Debug.Log("Wave " + currentWave + " finished. Next wave in " + timeBetweenWaves + " seconds.");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWaveGroup(EnemySpawner[] spawners)
    {
        if (spawners.Length == 0) yield break;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            foreach (EnemySpawner spawner in spawners)
            {
                if (spawner == null) continue;
                spawner.SpawnEnemyFromWave();
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
