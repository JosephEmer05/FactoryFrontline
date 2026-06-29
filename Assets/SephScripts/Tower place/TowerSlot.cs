using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool isOccupied = false;

    [Header("Height Settings")]
    public float placementHeight = 0f;

    [Header("Slot Colors")]
    public Color availableColor = Color.blue;
    public Color occupiedColor = Color.red;

    private MeshRenderer mr;
    private Color defaultColor;

    public GameObject currentTower;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        defaultColor = mr.material.color;

        TowerSelectionManager.Instance.RegisterSlot(this);
        UpdateColor();
        HideSlot();
    }

    public void AssignTower(GameObject tower)
    {
        currentTower = tower;
        isOccupied = true;
        UpdateColor();
    }

    public void ClearSlot()
    {
        currentTower = null;
        isOccupied = false;
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
        if (!isOccupied)
            mr.material.color = defaultColor;
        else
            mr.material.color = occupiedColor;
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
