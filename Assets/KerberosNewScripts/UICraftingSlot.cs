using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICraftingSlot : MonoBehaviour, IDropHandler
{
    public ComponentType slotType;
    public Image iconImage;
    public ComponentData currentPart;
    [HideInInspector] public TowerCrafter crafter;

    public void OnDrop(PointerEventData eventData)
    {
        UIDraggableItem draggedItem = eventData.pointerDrag.GetComponent<UIDraggableItem>();
        if (draggedItem != null && draggedItem.itemInstance != null)
        {
            ComponentData data = draggedItem.itemInstance.componentData;

            if (data.type == slotType)
            {
                SetPart(data);
                Destroy(draggedItem.gameObject);
            }
            else
            {
                Debug.LogWarning($"Wrong component type for this slot. Expected {slotType}, got {data.type}");
            }
        }
    }

    public void SetPart(ComponentData newPart)
    {
        currentPart = newPart;
        iconImage.sprite = newPart.icon;
        iconImage.color = Color.white;

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