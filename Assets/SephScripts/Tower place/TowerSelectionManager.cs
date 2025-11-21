using UnityEngine;
using System.Collections.Generic;

public class TowerSelectionManager : MonoBehaviour
{
    public static TowerSelectionManager Instance;

    private GameObject selectedTower;
    private List<TowerSlot> allSlots = new List<TowerSlot>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterSlot(TowerSlot slot)
    {
        allSlots.Add(slot);
        slot.HideSlot();
    }

    public void SelectTower(GameObject tower)
    {
        if (selectedTower == tower)
        {
            DeselectTower();
            return;
        }

        selectedTower = tower;
        ShowSlots();
    }

    public void DeselectTower()
    {
        selectedTower = null;
        HideSlots();
    }

    public void TryPlaceTower(TowerSlot slot)
    {
        if (selectedTower == null) return;

        if (!slot.isOccupied)
        {
            selectedTower.transform.position = slot.transform.position;
            slot.isOccupied = true;
            DeselectTower();
        }
    }

    private void ShowSlots()
    {
        foreach (var slot in allSlots)
            slot.ShowSlot();
    }

    private void HideSlots()
    {
        foreach (var slot in allSlots)
            slot.HideSlot();
    }
}
