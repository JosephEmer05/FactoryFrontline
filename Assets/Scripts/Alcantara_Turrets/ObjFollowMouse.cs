using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ObjFollowMouse : MonoBehaviour
{
    private DragGrid dragGrid;
    private Transform draggingInstance;
    private bool isDragging;

    // speed the ghost moves to the snapped grid position (higher = snappier)
    public float followSpeed = 12f;

    void Start()
    {
        dragGrid = FindFirstObjectByType<DragGrid>();
        if (dragGrid == null)
            Debug.LogWarning("ObjFollowMouse: No DragGrid found in scene.");
    }

    void Update()
    {
        if (!isDragging || draggingInstance == null) return;

        // Desired world target (smoothed by DragGrid)
        Vector3 desired = (dragGrid != null) ? dragGrid.smoothMousePosition : Vector3.zero;
        float gridY = (dragGrid != null) ? dragGrid.transform.position.y : 0f;
        desired.y = gridY;

        // Snap target to integer grid coords, but we lerp toward that snapped position for smooth motion
        Vector3 snappedTarget = (Vector3)Vector3Int.RoundToInt(desired);

        // Smooth follow
        draggingInstance.position = Vector3.Lerp(draggingInstance.position, snappedTarget, Mathf.Clamp01(Time.deltaTime * followSpeed));

        // Place with left click, cancel with right click
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                Place();
            else if (Mouse.current.rightButton.wasPressedThisFrame)
                Cancel();
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) Place();
            if (Input.GetMouseButtonDown(1)) Cancel();
        }
    }

    // Called by DragGrid to begin dragging a prefab ghost
    public void BeginDrag(Transform prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("ObjFollowMouse.BeginDrag: prefab is null.");
            return;
        }

        if (isDragging && draggingInstance != null)
            Destroy(draggingInstance.gameObject);

        draggingInstance = Instantiate(prefab);

        // disable physics (Rigidbody) if present, and disable colliders
        var rigidbodies = draggingInstance.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies) rb.isKinematic = true;

        var colliders = draggingInstance.GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = false;

        // make the ghost visually softer: turn off shadows and reduce alpha (works for Standard-like materials)
        var renderers = draggingInstance.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;

            // create unique material instances and lower alpha if possible
            for (int i = 0; i < r.materials.Length; i++)
            {
                var mat = r.materials[i]; // this creates an instance when accessed
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = Mathf.Min(c.a, 0.6f);
                    mat.color = c;
                }
            }
        }

        isDragging = true;

        if (dragGrid != null)
            dragGrid.onMousePrefab = draggingInstance;
    }

    private void Place()
    {
        if (draggingInstance == null) return;

        // re-enable colliders and restore physics if desired
        var colliders = draggingInstance.GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = true;

        var rigidbodies = draggingInstance.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies) rb.isKinematic = false;

        // optionally restore shadows — tutorial often leaves them off for a flat look
        var renderers = draggingInstance.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.shadowCastingMode = ShadowCastingMode.On;
            r.receiveShadows = true;
            // if you want to restore original alpha you must cache pre-drag materials; omitted for brevity
        }

        isDragging = false;
        draggingInstance = null;

        if (dragGrid != null)
            dragGrid.onMousePrefab = null;
    }

    private void Cancel()
    {
        if (draggingInstance != null)
            Destroy(draggingInstance.gameObject);

        isDragging = false;
        draggingInstance = null;

        if (dragGrid != null)
            dragGrid.onMousePrefab = null;
    }
}
