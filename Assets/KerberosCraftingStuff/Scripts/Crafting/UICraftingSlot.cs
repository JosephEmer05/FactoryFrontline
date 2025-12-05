using UnityEngine;
using UnityEngine.UI;

public class UICraftingSlot : MonoBehaviour
{
    public ComponentType slotType;
    //[SerializeField] private Image iconImage;

    public ComponentData partData;
    public GameObject currentPart;

    public bool hasPart;

    [HideInInspector] public TowerCraftingManager crafter;

    void OnEnable()   => CraftSlotManager.AddSlot(this);
    void OnDisable()  => CraftSlotManager.RemoveSlot(this);

    public Vector3 WorldPosition => transform.position; 

    public void SetPart(ComponentData part, GameObject partInstance)
    {
        partData = part;
        currentPart = partInstance;
        hasPart = true;

        //iconImage.sprite = part.icon;
        //iconImage.color = Color.white;

        crafter?.AddComponent(part);
    }

    public void ClearSlot()
    {
        Destroy(currentPart);
        currentPart = null;
        partData = null;
        hasPart = false;
        
        //iconImage.sprite = null;
        //iconImage.color = new Color(1,1,1,0);
    }   
}
