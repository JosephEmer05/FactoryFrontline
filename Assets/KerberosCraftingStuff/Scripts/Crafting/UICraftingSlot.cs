using UnityEngine;
using UnityEngine.UI;

public class UICraftingSlot : MonoBehaviour
{
    public ComponentType slotType;
    //[SerializeField] private Image iconImage;

    public ComponentData partData;
    public GameObject currentPart;

    [HideInInspector] public TowerCraftingManager crafter;

    void OnEnable()   => CraftSlotManager.AddSlot(this);
    void OnDisable()  => CraftSlotManager.RemoveSlot(this);

    public Vector3 WorldPosition => transform.position; 

    public void SetPart(ComponentData part, GameObject partInstance)
    {
        partData = part;
        currentPart = partInstance;

        //iconImage.sprite = part.icon;
        //iconImage.color = Color.white;

        crafter?.AddComponent(part);
    }

    public void ClearSlot()
    {
        Destroy(currentPart);
        currentPart = null;
        partData = null;
        
        //iconImage.sprite = null;
        //iconImage.color = new Color(1,1,1,0);
    }   
}
