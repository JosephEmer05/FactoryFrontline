using UnityEngine;
using UnityEngine.UI;

public class UICraftingSlot : MonoBehaviour
{
    public ComponentType slotType;
    public Image iconImage;
    public ComponentData currentPart;

    [HideInInspector] public TowerCrafter crafter;

    void Start()
    {
        ClearSlot();
    }

    public void SetPart(ComponentData newPart)
    {
        if (newPart.type != slotType)
        {
            Debug.LogWarning($"Invalid part type for this slot! Expected {slotType}, got {newPart.type}");
            return;
        }

        currentPart = newPart;
        iconImage.sprite = newPart.icon;
        iconImage.color = Color.white;

        // notify crafter
        if (crafter != null)
            crafter.AddComponent(newPart);
    }

    public void ClearSlot()
    {
        currentPart = null;
        iconImage.sprite = null;
        iconImage.color = new Color(1, 1, 1, 0);
    }
}
