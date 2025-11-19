using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    public Transform spawnPoint;
    public Transform endPoint;
    public float spawnInterval = 2f;
    public int maxItems = 6;

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
            Debug.LogWarning("ItemSpawner missing items or spawn point.");
            return;
        }

        int index = Random.Range(0, items.Length);
        GameObject newItem = Instantiate(items[index], spawnPoint.position, Quaternion.identity);

        MoveAlongConveyor moveScript = newItem.GetComponent<MoveAlongConveyor>();
        if (moveScript != null && endPoint != null)
        {
            moveScript.endPoint = endPoint;
        }
    }
}
