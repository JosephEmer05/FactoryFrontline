using UnityEngine;
using UnityEngine.UI;

public class RemoveButton : MonoBehaviour
{
    [SerializeField] private UICraftingSlot slot;
    private Button removeButton;

    void Start()
    {
        removeButton = GetComponent<Button>();

        removeButton.onClick.AddListener(slot.ClearSlot);
    }
}