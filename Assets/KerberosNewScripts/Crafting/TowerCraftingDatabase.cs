using UnityEngine;

public class TowerCraftingDatabase : MonoBehaviour
{
    public static TowerCraftingDatabase Instance;
    public TowerCombination[] towerCombinations;

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetCraftedTower(int baseID, int coreID, int weaponID)
    {
        foreach (TowerCombination combo in towerCombinations)
        {
            if (combo.baseID == baseID && combo.coreID == coreID && combo.weaponID == weaponID)
            {
                return combo.towerPrefab;
            }
        }
        Debug.LogWarning("No tower combination found for Base:" + baseID + " Core:" + coreID + " Weapon:" + weaponID);
        return null;
    }
}
