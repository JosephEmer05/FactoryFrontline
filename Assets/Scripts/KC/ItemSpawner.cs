using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items; //
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    private float timer;

    void Update()
    {
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
        Instantiate(items[index], spawnPoint.position, Quaternion.identity);
    }
}
