using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class WaveEntry
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float spawnDelay = 0.5f;
    public EnemySpawner[] specificSpawners;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public WaveEntry[] enemies;
    public bool useLowSpawners = true;
    public bool useHighSpawners = false;
    public float timeAfterWave = 8f;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public EnemySpawner[] lowSpawners;
    public EnemySpawner[] highSpawners;

    public Wave[] waves;

    private int currentWave = -1;

    private int totalEnemiesAllWaves = 0;
    private int enemiesRemaining = 0;
    private int enemiesRemainingAtWaveStart = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        totalEnemiesAllWaves = CalculateTotalEnemiesAcrossAllWaves();
        enemiesRemaining = totalEnemiesAllWaves;

        if (UIManager.Instance != null)
            UIManager.Instance.SetEnemySlider(totalEnemiesAllWaves, enemiesRemaining);

        StartCoroutine(WaveRoutine());
    }

    int CalculateTotalEnemiesAcrossAllWaves()
    {
        int total = 0;
        foreach (var wave in waves)
        {
            foreach (var entry in wave.enemies)
            {
                int spawnerCount = 0;

                if (entry.specificSpawners != null && entry.specificSpawners.Length > 0)
                    spawnerCount = entry.specificSpawners.Length;
                else
                {
                    if (wave.useLowSpawners) spawnerCount += lowSpawners.Length;
                    if (wave.useHighSpawners) spawnerCount += highSpawners.Length;
                }

                total += entry.count * Mathf.Max(1, spawnerCount);
            }
        }
        return total;
    }

    public void OnEnemyDied()
    {
        enemiesRemaining = Mathf.Max(0, enemiesRemaining - 1);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEnemySlider(enemiesRemaining);
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < waves.Length; i++)
        {
            currentWave = i;
            Wave wave = waves[i];

            float timer = wave.timeAfterWave;

            if (i == 0)
            {
                while (timer > 0f)
                {
                    if (UIManager.Instance != null)
                        UIManager.Instance.UpdateWaveTimer(timer);

                    yield return null;
                    timer -= Time.deltaTime;
                }

                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateWaveTimer(0);
            }
            else
            {
                while (enemiesRemaining > enemiesRemainingAtWaveStart)
                    yield return null;

                while (timer > 0f)
                {
                    if (UIManager.Instance != null)
                        UIManager.Instance.UpdateWaveTimer(timer);

                    yield return null;
                    timer -= Time.deltaTime;
                }

                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateWaveTimer(0);
            }

            enemiesRemainingAtWaveStart = enemiesRemaining;

            foreach (var entry in wave.enemies)
            {
                for (int c = 0; c < entry.count; c++)
                {
                    EnemySpawner[] chosen = null;

                    if (entry.specificSpawners != null && entry.specificSpawners.Length > 0)
                        chosen = entry.specificSpawners;
                    else
                        chosen = (wave.useLowSpawners ? lowSpawners : highSpawners);

                    foreach (var sp in chosen)
                    {
                        if (sp == null) continue;
                        sp.SpawnEnemy(entry.enemyPrefab);
                    }

                    yield return new WaitForSeconds(entry.spawnDelay);
                }
            }
        }
    }

    public int GetCurrentWave() => currentWave + 1;
}
