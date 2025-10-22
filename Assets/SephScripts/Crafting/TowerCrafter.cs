using UnityEngine;

public class TowerCrafter : MonoBehaviour
{
    [Header("Crafting Slots")]
    public ComponentData baseSlot;
    public ComponentData coreSlot;
    public ComponentData weaponSlot;

    [Header("Crafting Output")]
    public Transform spawnPoint;
    public bool autoClearAfterCraft = true;

    public void AddComponent(ComponentData component)
    {
        switch (component.type)
        {
            case ComponentType.Base:
                baseSlot = component;
                break;
            case ComponentType.Core:
                coreSlot = component;
                break;
            case ComponentType.Weapon:
                weaponSlot = component;
                break;
        }

        TryCraft();
    }

    void TryCraft()
    {
        if (baseSlot != null && coreSlot != null && weaponSlot != null)
        {
            GameObject towerPrefab = TowerCraftingDatabase.Instance.GetCraftedTower(
                baseSlot.componentID,
                coreSlot.componentID,
                weaponSlot.componentID
            );

            if (towerPrefab != null)
            {
                Instantiate(towerPrefab, spawnPoint.position, Quaternion.identity);
                Debug.Log($"Crafted Tower from Base:{baseSlot.name}, Core:{coreSlot.name}, Weapon:{weaponSlot.name}");
            }

            if (autoClearAfterCraft)
            {
                baseSlot = null;
                coreSlot = null;
                weaponSlot = null;
            }
        }
    }
}
