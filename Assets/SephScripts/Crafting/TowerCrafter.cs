using UnityEngine;

public class TowerCrafter : MonoBehaviour
{
    [Header("UI Slots")]
    public UICraftingSlot baseSlotUI;
    public UICraftingSlot coreSlotUI;
    public UICraftingSlot weaponSlotUI;

    [Header("Crafting Output")]
    public Transform spawnPoint;
    public bool autoClearAfterCraft = true;

    void Start()
    {
        // let slots know who their parent is
        baseSlotUI.crafter = this;
        coreSlotUI.crafter = this;
        weaponSlotUI.crafter = this;
    }

    public void AddComponent(ComponentData component)
    {
        switch (component.type)
        {
            case ComponentType.Base:
                baseSlotUI.currentPart = component;
                break;
            case ComponentType.Core:
                coreSlotUI.currentPart = component;
                break;
            case ComponentType.Weapon:
                weaponSlotUI.currentPart = component;
                break;
        }

        TryCraft();
    }

    void TryCraft()
    {
        if (baseSlotUI.currentPart != null && coreSlotUI.currentPart != null && weaponSlotUI.currentPart != null)
        {
            GameObject towerPrefab = TowerCraftingDatabase.Instance.GetCraftedTower(
                baseSlotUI.currentPart.componentID,
                coreSlotUI.currentPart.componentID,
                weaponSlotUI.currentPart.componentID
            );

            if (towerPrefab != null)
                Instantiate(towerPrefab, spawnPoint.position, Quaternion.identity);

            if (autoClearAfterCraft)
            {
                baseSlotUI.ClearSlot();
                coreSlotUI.ClearSlot();
                weaponSlotUI.ClearSlot();
            }
        }
    }
}
