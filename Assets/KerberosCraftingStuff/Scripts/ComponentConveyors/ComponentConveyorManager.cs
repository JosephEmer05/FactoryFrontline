using UnityEngine;
using System.Collections.Generic;

public class ComponentConveyorManager : MonoBehaviour
{
    [Header("Conveyor Config")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform componentHolder;

    [Header("Component Config")]
    [SerializeField] GameObject[] towerComponents;
    [SerializeField] private int maxItems;
    [SerializeField] private float spawnInterval;
    [Space]
    [SerializeField] private Vector3 itemSpacing;
    [SerializeField] private float itemSpeed;

    private float timer;

    private List<ComponentInstance> items = new List<ComponentInstance>();

    void Update()
    {
        StartSpawning();
        UpdateComponentPositions();
    }

    // Component Spawning
    private void StartSpawning()
    {
        timer += Time.deltaTime;

        if (items.Count >= maxItems) return;

        if (timer >= spawnInterval)
        {
            SpawnComponent();
            timer = 0f;
        }
    }

    private void SpawnComponent()
    {
        // Randomize Component Type
        int i = Random.Range(0, towerComponents.Length);

        GameObject spawnedItem = Instantiate(towerComponents[i], startPoint.position, Quaternion.identity, componentHolder);
        ComponentInstance instance = spawnedItem.GetComponent<ComponentInstance>();
        instance.conveyor = this;
        AddComponent(instance);
    }

    public void AddComponent(ComponentInstance item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }

        SortByPosition();
    }

    public void RemoveComponent(ComponentInstance item)
    {
        items.Remove(item);

        SortByPosition();
        UpdateComponentPositions();
    }

    void SortByPosition()
    {
        items.Sort((a, b) =>
        {
            if (a.isDragging && b.isDragging) return 0;
            if (a.isDragging) return 0;
            if (b.isDragging) return 0;

            float aDist = Vector3.Distance(a.transform.position, endPoint.position);
            float bDist = Vector3.Distance(b.transform.position, endPoint.position);
            return aDist.CompareTo(bDist);
        });
    }

    void UpdateComponentPositions()
    {
        Vector3 targetPos = endPoint.position;

        for (int i = 0; i < items.Count; i++)
        {
            ComponentInstance item = items[i];

            if (item.isDragging)
            {
                // Keep targetPos unchanged for next items
                targetPos -= new Vector3(itemSpacing.x, itemSpacing.y, itemSpacing.z);
                continue;
            }

            item.transform.position = Vector3.MoveTowards(item.transform.position, targetPos, itemSpeed * Time.deltaTime);
            item.originalPos = targetPos;

            targetPos -= new Vector3(itemSpacing.x, itemSpacing.y, itemSpacing.z);
        }
    }
}
