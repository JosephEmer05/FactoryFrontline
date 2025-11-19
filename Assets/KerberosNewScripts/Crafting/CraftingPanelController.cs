using UnityEngine;
using UnityEngine.UI;

public class CraftingPanelController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform panel;
    public Button tabButton;

    [Header("Animation Settings")]
    public float openX = 200f;
    public float closedX = -600f;
    public float speed = 10f;

    private bool isOpen = false;
    private Vector2 targetPos;

    void Start()
    {
        targetPos = new Vector2(closedX, panel.anchoredPosition.y);
        panel.anchoredPosition = targetPos;

        if (tabButton != null)
            tabButton.onClick.AddListener(TogglePanel);
    }

    void Update()
    {
        panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, targetPos, Time.unscaledDeltaTime * speed);
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;
        targetPos = new Vector2(isOpen ? openX : closedX, panel.anchoredPosition.y);
    }
}
