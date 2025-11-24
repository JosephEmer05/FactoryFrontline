using System.Collections;
using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float spawnDelay = 0.5f;

    [Header("Specific Spawners For This Enemy Type (Optional)")]
    public EnemySpawner[] specificSpawners;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public WaveEntry[] enemies;

    [Header("Fallback Spawner Settings")]
    public bool useLowSpawners = true;
    public bool useHighSpawners = false;

    public float timeAfterWave = 8f;
}

public class WaveManager : MonoBehaviour
{
    [Header("Spawner References")]
    public EnemySpawner[] lowSpawners;
    public EnemySpawner[] highSpawners;

    [Header("Waves Configuration")]
    public Wave[] waves;

    private int currentWave = -1;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < waves.Length; i++)
        {
            currentWave = i;
            Wave wave = waves[i];

            Debug.Log($"--- Starting {wave.waveName} ---");

            yield return new WaitForSeconds(wave.timeAfterWave);

            yield return StartCoroutine(SpawnWave(wave));

            Debug.Log($"--- {wave.waveName} finished ---");
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (WaveEntry entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                if (entry.specificSpawners != null && entry.specificSpawners.Length > 0)
                {
                    foreach (EnemySpawner spawner in entry.specificSpawners)
                    {
                        if (spawner != null)
                            spawner.SpawnEnemy(entry.enemyPrefab);
                    }
                }
                else
                {
                    if (wave.useLowSpawners)
                    {
                        foreach (EnemySpawner spawner in lowSpawners)
                        {
                            if (spawner != null)
                                spawner.SpawnEnemy(entry.enemyPrefab);
                        }
                    }

                    if (wave.useHighSpawners)
                    {
                        foreach (EnemySpawner spawner in highSpawners)
                        {
                            if (spawner != null)
                                spawner.SpawnEnemy(entry.enemyPrefab);
                        }
                    }
                }

                yield return new WaitForSeconds(entry.spawnDelay);
            }
        }
    }

    public int GetCurrentWave()
    {
        return currentWave + 1;
    }
}
