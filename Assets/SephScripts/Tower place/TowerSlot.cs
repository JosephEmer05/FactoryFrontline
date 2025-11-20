using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool isOccupied = false;

    public bool PlaceTower(GameObject tower)
    {
        if (isOccupied)
        {
            Debug.Log("Slot already occupied.");
            return false;
        }

        tower.transform.position = transform.position;
        isOccupied = true;
        return true;
    }
}
