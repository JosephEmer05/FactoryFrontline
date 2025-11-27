using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool isOccupied = false;

    [Header("Height Settings")]
    public float placementHeight = 0f;

    private MeshRenderer mr;
    private Color defaultColor;

    public Color availableColor = Color.blue;
    public Color occupiedColor = Color.red;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        defaultColor = mr.material.color;

        TowerSelectionManager.Instance.RegisterSlot(this);
        UpdateColor();
    }

    public void ShowSlot()
    {
        mr.enabled = true;
        UpdateColor();
    }

    public void HideSlot()
    {
        mr.enabled = false;
    }

    public void UpdateColor()
    {
        mr.material.color = isOccupied ? occupiedColor : defaultColor;
    }

    private void OnMouseEnter()
    {
        if (!mr.enabled) return;

        if (!isOccupied)
            mr.material.color = availableColor;
        else
            mr.material.color = occupiedColor;
    }

    private void OnMouseExit()
    {
        if (!mr.enabled) return;

        UpdateColor();
    }

    private void OnMouseDown()
    {
        if (!mr.enabled) return;

        TowerSelectionManager.Instance.TryPlaceTower(this);
    }
}
