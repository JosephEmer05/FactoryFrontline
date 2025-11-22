using UnityEngine;
using UnityEngine.UI;

public class TowerCraftingManager : MonoBehaviour
{
    [Header("Recipe Data")]
    [SerializeField] private RecipeData recipeData;

    [Header("UI Assignments")]
    [SerializeField] private Button craftButton;
    [SerializeField] private UICraftingSlot weaponSlotUI;
    [SerializeField] private UICraftingSlot coreSlotUI;
    [SerializeField] private UICraftingSlot baseSlotUI;

    [Header("Output Config")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform packageHolder;
    [SerializeField] private PackageConveyorManager packageConveyor;
    //public bool autoClearAfterCraft = true;

    void Start()
    {
        baseSlotUI.crafter = this;
        coreSlotUI.crafter = this;
        weaponSlotUI.crafter = this;

        craftButton.onClick.AddListener(CraftTower);
    }

    public void AddComponent(ComponentData component)
    {
        switch (component.type)
        {
            case ComponentType.Base:
                baseSlotUI.partData = component;
                break;
            case ComponentType.Core:
                coreSlotUI.partData = component;
                break;
            case ComponentType.Weapon:
                weaponSlotUI.partData = component;
                break;
        }
    }

    private void CraftTower()
    {
        if (baseSlotUI.partData == null || 
            coreSlotUI.partData == null ||
            weaponSlotUI.partData == null)
        {
            // !! Debug
            Debug.Log("Missing a component");
            return;
        }

        GameObject packagePrefab = GetTower(
            baseSlotUI.partData.componentID,
            coreSlotUI.partData.componentID,
            weaponSlotUI.partData.componentID
        );

        if (packagePrefab != null)
        {
            if (spawnPoint == null)
            {
                // !! Debug
                Debug.LogWarning("Missing spawn point");
                return;
            }

            //conveyor.SpawnTower(packagePrefab);
            GameObject spawnedItem = Instantiate(packagePrefab, spawnPoint.position, Quaternion.identity, packageHolder);
            PackageInstance instance = spawnedItem.GetComponent<PackageInstance>();
            instance.conveyor = packageConveyor;

            // !! Debug
            Debug.Log("Successfully crafted: " + packagePrefab);
        }

        baseSlotUI.ClearSlot();
        coreSlotUI.ClearSlot();
        weaponSlotUI.ClearSlot();
    }

    private GameObject GetTower(int baseID, int coreID, int weapID)
    {
        foreach (var recipe in recipeData.recipes)
        {
            if (weapID == recipe.weapID &&
                coreID == recipe.coreID &&
                baseID == recipe.baseID)
            {
                // !! Debug
                Debug.Log("Found tower.");

                return recipe.packagePrefab;
            }
        }

        // !! Debug
        Debug.Log("Tower couldn't be found.");

        return null;
    }
}
