using UnityEngine;
using System.Collections.Generic;

public class TowerOutputConveyor : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public Transform spawnPoint;
    public Transform endPoint;
    public float moveSpeed = 2f;
    public float spacing = 1.5f;
    public LayerMask towerLayer;

    private List<GameObject> activeTowers = new List<GameObject>();

    public void SpawnTower(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning("Tower prefab missing!");
            return;
        }

        GameObject newTower = Instantiate(towerPrefab, spawnPoint.position, Quaternion.identity);
        activeTowers.Add(newTower);
        TowerConveyorMover mover = newTower.AddComponent<TowerConveyorMover>();
        mover.Initialize(this, endPoint.position, moveSpeed, spacing, towerLayer);
    }

    public void RemoveTower(GameObject tower)
    {
        if (activeTowers.Contains(tower))
            activeTowers.Remove(tower);
    }
}
