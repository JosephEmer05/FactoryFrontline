using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Wave UI")]
    public Slider waveSlider;
    public Text waveCounterText;

    [Header("Win UI")]
    public GameObject winUI;

    private int totalEnemies = 0;
    private int aliveEnemies = 0;

    void Start()
    {
        if (winUI != null)
            winUI.SetActive(false);

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

            CountWaveEnemies(wave);

            UpdateUI();

            yield return StartCoroutine(SpawnWave(wave));

            // Wait until all enemies die
            yield return new WaitUntil(() => aliveEnemies <= 0);

            Debug.Log($"--- {wave.waveName} finished ---");
        }

        // Show win screen
        if (winUI != null)
            winUI.SetActive(true);

        Debug.Log("All waves completed!");
    }

    void CountWaveEnemies(Wave wave)
    {
        totalEnemies = 0;

        foreach (WaveEntry entry in wave.enemies)
        {
            // Count how many spawners will be used
            int spawnerCount = 0;

            if (entry.specificSpawners != null && entry.specificSpawners.Length > 0)
            {
                spawnerCount = entry.specificSpawners.Length;
            }
            else
            {
                if (wave.useLowSpawners)
                    spawnerCount += lowSpawners.Length;
                if (wave.useHighSpawners)
                    spawnerCount += highSpawners.Length;
            }

            totalEnemies += entry.count * spawnerCount;
        }

        aliveEnemies = totalEnemies;

        if (waveSlider != null)
        {
            waveSlider.maxValue = totalEnemies;
            waveSlider.value = totalEnemies;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (WaveEntry entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                // Specific spawners
                if (entry.specificSpawners != null && entry.specificSpawners.Length > 0)
                {
                    foreach (EnemySpawner spawner in entry.specificSpawners)
                    {
                        if (spawner != null)
                        {
                            GameObject spawned = spawner.SpawnEnemy(entry.enemyPrefab);
                            RegisterSpawn(spawned);
                        }
                    }
                }
                else
                {
                    // Use fallback low spawners
                    if (wave.useLowSpawners)
                    {
                        foreach (EnemySpawner spawner in lowSpawners)
                        {
                            if (spawner != null)
                            {
                                GameObject spawned = spawner.SpawnEnemy(entry.enemyPrefab);
                                RegisterSpawn(spawned);
                            }
                        }
                    }

                    // Use fallback high spawners
                    if (wave.useHighSpawners)
                    {
                        foreach (EnemySpawner spawner in highSpawners)
                        {
                            if (spawner != null)
                            {
                                GameObject spawned = spawner.SpawnEnemy(entry.enemyPrefab);
                                RegisterSpawn(spawned);
                            }
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

    private void RegisterSpawn(GameObject enemyObj)
    {
        if (enemyObj == null) return;

        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.onEnemyDied += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(BaseEnemy enemy)
    {
        aliveEnemies--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (waveSlider != null)
            waveSlider.value = aliveEnemies;

        if (waveCounterText != null)
            waveCounterText.text = $"{aliveEnemies}/{totalEnemies}";
    }
}
