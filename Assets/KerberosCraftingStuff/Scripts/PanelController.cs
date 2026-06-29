using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform UICraftingPanel;
    [SerializeField] private Button UICraftingButton;
    [Space]
    [SerializeField] private Toggle UIPinToggle;
    public bool isPinned;

    [Header("Animation Settings")]
    public float showPosX = 200f;
    public float hidePosX = -600f;
    private float lerpSpeed = 10f;

    private bool isOpen = false;
    private Vector2 targetPos;

    void Start()
    {
        targetPos = new Vector2(hidePosX, UICraftingPanel.anchoredPosition.y);
        UICraftingPanel.anchoredPosition = targetPos;

        if (UICraftingButton != null)
        {
            UICraftingButton.onClick.AddListener(TogglePanel);
        }

        if (UIPinToggle != null)
        {
            UIPinToggle.onValueChanged.AddListener(TogglePin);
        }
    }

    void Update()
    {
        UICraftingPanel.anchoredPosition = Vector2.Lerp(UICraftingPanel.anchoredPosition, targetPos, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;
        targetPos = new Vector2(isOpen ? showPosX : hidePosX, UICraftingPanel.anchoredPosition.y);
    }

    public void TogglePin(bool value)
    {
        isPinned = value;
    }
}
