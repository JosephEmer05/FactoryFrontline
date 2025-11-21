using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TowerOutputConveyor : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform endPoint;
    public float moveSpeed = 2f;
    public float spacing = 1.5f;
    public LayerMask towerLayer;
    public GameObject[] towerPrefabs;
    public Key spawnKey = Key.T;

    private List<GameObject> activeTowers = new List<GameObject>();

    void Update()
    {
        if (Keyboard.current[spawnKey].wasPressedThisFrame)
        {
            if (towerPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, towerPrefabs.Length);
                SpawnTower(towerPrefabs[randomIndex]);
            }
        }
    }

    public void SpawnTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;
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
