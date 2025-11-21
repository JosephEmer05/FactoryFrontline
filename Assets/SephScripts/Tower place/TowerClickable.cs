using UnityEngine;

public class TowerClickable : MonoBehaviour
{
    private void OnMouseDown()
    {
        TowerSelectionManager.Instance.SelectTower(gameObject);
    }
}
