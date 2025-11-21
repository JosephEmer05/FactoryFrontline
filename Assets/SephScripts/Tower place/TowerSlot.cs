using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool isOccupied = false;

    private MeshRenderer mr;
    private Color defaultColor;

    public Color availableColor = Color.blue;
    public Color occupiedColor = Color.red;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        defaultColor = mr.material.color;

        TowerSelectionManager.Instance.RegisterSlot(this);
    }

    public void ShowSlot()
    {
        mr.enabled = true;
    }

    public void HideSlot()
    {
        mr.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (!mr.enabled) return;

        mr.material.color = isOccupied ? occupiedColor : availableColor;
    }

    private void OnMouseExit()
    {
        if (!mr.enabled) return;

        mr.material.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (!mr.enabled) return;

        TowerSelectionManager.Instance.TryPlaceTower(this);
    }
}
