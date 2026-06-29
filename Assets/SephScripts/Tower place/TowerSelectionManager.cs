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
        if (!allSlots.Contains(slot))
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
        if (slot == null) return;
        if (slot.isOccupied) return;

        PackageInstance pkg = selectedTower.GetComponent<PackageInstance>();
        if (pkg == null)
        {
            Debug.LogWarning("Selected object has no PackageInstance - cannot place.");
            return;
        }

        Vector3 slotPos = slot.transform.position;
        selectedTower.transform.position = new Vector3(
            slotPos.x,
            slot.placementHeight,
            slotPos.z
        );

        pkg.conveyor.RemovePackage(pkg);

        slot.AssignTower(selectedTower);

        TestTower towerComp = selectedTower.GetComponent<TestTower>();
        if (towerComp != null)
        {
            towerComp.assignedSlot = slot;
        }
        DeselectTower();
    }

    private void ShowSlots()
    {
        foreach (var slot in allSlots)
            if (!slot.isOccupied)
                slot.ShowSlot();
    }

    private void HideSlots()
    {
        foreach (var slot in allSlots)
            slot.HideSlot();
    }
}
