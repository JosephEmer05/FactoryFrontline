using UnityEngine;

public class TilemapPlacementManager : MonoBehaviour
{
    public GameObject prefabToPlace;
    public TilemapGridManager gridManager;
    private GameObject previewObject;
    private Camera mainCamera;

    public Material transparentMaterial;  //Transparent material for preview object

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleDragging();
    }

    private void HandleDragging()
    {
        if (previewObject == null)
        {
            if (Input.GetMouseButtonDown(0)) //LMB to start dragging
            {
                Vector3 mousePos = GetMouseWorldPosition();
                previewObject = Instantiate(prefabToPlace, mousePos, Quaternion.identity);

                //Apply transparent material to the preview object
                Renderer renderer = previewObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = transparentMaterial;
                    Color color = renderer.material.color;
                    color.a = 0.5f;
                    renderer.material.color = color;
                }
            }
        }
        else
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3Int gridPosition = gridManager.GetGridPosition(mousePos);
            Vector3 snappedPosition = gridManager.GetTileWorldPosition(gridPosition);

            previewObject.transform.position = snappedPosition;

            bool isAvailable = !gridManager.IsTileBlocked(snappedPosition);

            //Changes the preview color based on availability of tile pos
            Renderer previewRenderer = previewObject.GetComponent<Renderer>();
            if (isAvailable)
            {
                previewRenderer.material.color = new Color(0f, 1f, 0f, 0.5f);  //Green for available
            }
            else
            {
                previewRenderer.material.color = new Color(1f, 0f, 0f, 0.5f);  //Red for blocked
            }

            if (Input.GetMouseButtonDown(1)) //RMB to cancel placement (for testing)
            {
                Destroy(previewObject);
                previewObject = null;
            }

            if (Input.GetMouseButtonUp(0)) //LMB to place the object after player tries to put object on blocked tile
            {
                if (isAvailable)
                {
                    PlaceObject(gridPosition);
                }
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    private void PlaceObject(Vector3Int gridPosition)
    {
        previewObject.transform.position = gridManager.GetTileWorldPosition(gridPosition);
        Instantiate(prefabToPlace, previewObject.transform.position, Quaternion.identity);
        Destroy(previewObject);
        previewObject = null;
    }
}
