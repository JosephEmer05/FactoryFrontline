using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int maxItems = 6;

    private float timer;
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Update()
    {
        //Clean up any destroyed items (to handle when items are removed in-game)
        spawnedItems.RemoveAll(item => item == null);

        // Pause spawning if limit reached
        if (spawnedItems.Count >= maxItems)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        if (items.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("ItemSpawner is missing item prefabs or spawn point.");
            return;
        }

        int index = Random.Range(0, items.Length);
        GameObject newItem = Instantiate(items[index], spawnPoint.position, Quaternion.identity);
        spawnedItems.Add(newItem);
    }
}