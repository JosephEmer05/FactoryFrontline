using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragGrid : MonoBehaviour
{
    public Transform gridCellPrefab;
    public Transform cube;                // prefab to spawn (world-space)
    public Transform onMousePrefab;
    public Canvas uiCanvas;
    public Vector3 smoothMousePosition;
    [SerializeField] private int height;
    [SerializeField] private int width;

    public LayerMask groundLayer;

    // New: smoothing speed for mouse world follow (higher = snappier)
    [Tooltip("Higher = faster follow toward the raw mouse position")]
    public float followLerp = 15f;

    private Vector3 mousePosition;
    private Node[,] nodes;
    private Plane plane;

    void Start()
    {
        CreateGrid();
        plane = new Plane(inNormal: Vector3.up, inPoint: transform.position);

        // initialize smooth position to avoid big first-frame jumps
        smoothMousePosition = transform.position;
    }

    void Update()
    {
        GetMousePositionOnGrid();
    }

    void GetMousePositionOnGrid()
    {
        if (Camera.main == null) return;

        Vector2 mousePos;
        if (Mouse.current != null) mousePos = Mouse.current.position.ReadValue();
        else mousePos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (plane.Raycast(ray, out var enter) && enter > 0f)
        {
            mousePosition = ray.GetPoint(enter);
            mousePosition.y = transform.position.y;
            mousePosition = (Vector3)Vector3Int.RoundToInt(mousePosition);

            // Smooth the mouse world position to avoid instant snapping
            smoothMousePosition = Vector3.Lerp(smoothMousePosition, mousePosition, Mathf.Clamp01(Time.deltaTime * followLerp));
        }
    }

    // Called by UI button to start dragging the selected prefab
    public void OnMouseClickOnUI()
    {
        if (cube == null)
        {
            Debug.LogWarning("DragGrid: 'cube' prefab is not assigned.");
            return;
        }

        StartDrag(cube);
    }

    // Request ObjFollowMouse to begin dragging the prefab
    public void StartDrag(Transform prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("DragGrid.StartDrag: prefab is null.");
            return;
        }

        var follower = FindFirstObjectByType<ObjFollowMouse>();
        if (follower == null)
        {
            Debug.LogWarning("DragGrid.StartDrag: no ObjFollowMouse instance found in scene. Add ObjFollowMouse to a GameObject.");
            return;
        }

        follower.BeginDrag(prefab);
    }

    private void CreateGrid()
    {
        nodes = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 worldPosition = new Vector3(i, 0, j);
                Transform obj = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity);
                obj.name = "Cell " + name;
                nodes[i, j] = new Node(true, worldPosition, obj);
            }
        }
    }

    public class Node
    {
        public bool isPlaceable;
        public Vector3 cellPosition;
        public Transform obj;

        public Node(bool isPlaceable, Vector3 cellPosition, Transform obj)
        {
            this.isPlaceable = isPlaceable;
            this.cellPosition = cellPosition;
            this.obj = obj;
        }
    }
}

