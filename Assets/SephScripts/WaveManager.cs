using System.Collections;
using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float spawnDelay = 0.5f;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public WaveEntry[] enemies;
    public bool useLowSpawners = true;
    public bool useHighSpawners = false;
    public float timeAfterWave = 8f;  // Delay after wave finishes before starting next wave
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
        yield return new WaitForSeconds(2f);  // Initial delay before first wave starts

        for (int i = 0; i < waves.Length; i++)
        {
            currentWave = i;
            Wave wave = waves[i];

            Debug.Log($"--- Starting {wave.waveName} ---");

            // Wait for the 'timeAfterWave' before actually spawning the enemies
            yield return new WaitForSeconds(wave.timeAfterWave);  // Delay before spawning begins

            // Start spawning the wave
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

                // Delay between enemy spawns
                yield return new WaitForSeconds(entry.spawnDelay);
            }
        }
    }

    public int GetCurrentWave()
    {
        return currentWave + 1;
    }
}
