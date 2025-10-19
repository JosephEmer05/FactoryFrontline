using UnityEngine;
using UnityEngine.InputSystem;  

public class DragGrid : MonoBehaviour
{
    public Transform gridCellPrefab;
    public Transform cube;

    public Vector3 smoothMousePosition;
    [SerializeField] private int height;
    [SerializeField] private int width;

    private Vector3 mousePosition;
    private Node[,] nodes;
    private Plane plane;

    void Start()
    {
        CreateGrid();
        plane = new Plane(inNormal: Vector3.up, inPoint: transform.position);
    }

    void Update()
    {
        GetMousePositionOnGrid();
    }
    void GetMousePositionOnGrid()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (plane.Raycast(ray, out var enter))
        {
            mousePosition = ray.GetPoint(enter);
            smoothMousePosition = mousePosition;
            mousePosition.y = 0;
            mousePoisition = Vector3Int.RoundToInt(mousePosition);
        }
    }


    private void CreateGrid()
    {
        nodes = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 worldPosition = new Vector3(x: i, y: 0, z: j);
                Transform obj = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity);
                obj.name = "Cell " + name;
                nodes[i, j] = new Node(isPlaceable: true, worldPosition, obj);
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

