using UnityEngine;
using UnityEngine.UI;

public class UICraftingSlot : MonoBehaviour
{
    public bool hasPart;
    public ComponentType slotType;
    [SerializeField] private Button ClearSlotButton;
    //[SerializeField] private Image iconImage;

    public ComponentData partData;
    public GameObject currentPart;

    [HideInInspector] public TowerCraftingManager crafter;

    void Awake()
    {
        if (ClearSlotButton != null)
        {
            ClearSlotButton.onClick.RemoveAllListeners();
            ClearSlotButton.onClick.AddListener(ClearSlot);
            ClearSlotButton.interactable = hasPart;
        }
    }

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

        if (ClearSlotButton != null) ClearSlotButton.interactable = true;

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

        if (ClearSlotButton != null) ClearSlotButton.interactable = false;
    }   
}
