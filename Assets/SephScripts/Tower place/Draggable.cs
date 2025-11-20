using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Prefab to Spawn on Drag")]
    public GameObject towerPrefab;

    private GameObject dragPreview;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Spawn preview copy of the tower
        dragPreview = Instantiate(towerPrefab);
        dragPreview.GetComponent<Collider2D>().enabled = false; // Disable collisions while dragging
        dragPreview.layer = LayerMask.NameToLayer("IgnoreRaycast");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0f;
        dragPreview.transform.position = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 dropPosition = Camera.main.ScreenToWorldPoint(eventData.position);

        Collider2D hit = Physics2D.OverlapPoint(dropPosition);

        // Check if dropped over a slot
        if (hit != null && hit.CompareTag("TowerSlot"))
        {
            TowerSlot slot = hit.GetComponent<TowerSlot>();

            if (slot != null && slot.PlaceTower(dragPreview))
            {
                dragPreview.GetComponent<Collider2D>().enabled = true;
                dragPreview.layer = 0;
                dragPreview = null;
                return;
            }
        }

        // If invalid slot → destroy preview
        Destroy(dragPreview);
    }
}
