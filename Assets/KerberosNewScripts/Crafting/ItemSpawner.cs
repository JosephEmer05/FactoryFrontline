using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    public Transform spawnPoint;
    public Transform endPoint;
    public float spawnInterval = 2f;
    public int maxItems = 8;

    private float timer;

    public List<MoveAlongConveyor> activeItems = new List<MoveAlongConveyor>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnItem();
            timer = 0f;
        }
    }

    void TrySpawnItem()
    {
        if (activeItems.Count >= maxItems) return;

        GameObject prefab = items[Random.Range(0, items.Length)];
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        if (endPoint != null)
            obj.transform.rotation = Quaternion.LookRotation(endPoint.position - spawnPoint.position);

        MoveAlongConveyor move = obj.GetComponent<MoveAlongConveyor>();
        if (move != null)
        {
            move.ownerSpawner = this;
            move.endPoint = endPoint;
            activeItems.Add(move);
        }
    }

    public void PauseAll()
    {
        foreach (var i in activeItems)
            if (i != null) i.Pause();
    }

    public void ResumeAll()
    {
        foreach (var i in activeItems)
            if (i != null) i.Resume();
    }

    public void RemoveItem(MoveAlongConveyor item)
    {
        activeItems.Remove(item);
    }
}
