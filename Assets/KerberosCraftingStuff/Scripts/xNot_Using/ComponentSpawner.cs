using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    [SerializeField] private Transform componentHolder;
    [Space]
    [SerializeField] private GameObject[] towerComponents;
    [Space]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endPoint;

    private float spawnInterval = 2f;
    private int maxItems = 6;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (componentHolder.childCount >= maxItems)
        {
            return;
        }

        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        // Randomize Which Type to Instantiate
        int i = Random.Range(0, towerComponents.Length);

        GameObject spawnedComponent = Instantiate(towerComponents[i], spawnPoint.position, Quaternion.identity, componentHolder);
        var instance = spawnedComponent.GetComponent<ComponentInstance>();
        instance.conveyor = GetComponent<ComponentConveyorManager>();
        instance.conveyor.AddComponent(instance);
    }
}
