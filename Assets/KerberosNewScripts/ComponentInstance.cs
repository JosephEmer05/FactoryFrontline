using UnityEngine;
using UnityEngine.InputSystem;

public class ComponentInstance : MonoBehaviour
{
    private Camera mainCamera;
    private MoveAlongConveyor moveScript;

    private bool isDragging = false;
    private float zOffset;
    private Vector3 offset;
    private Vector3 originalPosition;

    public ComponentData componentData;

    void Awake()
    {
        mainCamera = Camera.main;
        moveScript = GetComponent<MoveAlongConveyor>();
    }

    void Update()
    {
        var mouse = Mouse.current;

        // Register Click
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    moveScript.SetStatus(false);

                    originalPosition = transform.position;

                    zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
                    Vector3 mousePosition = mouse.position.ReadValue();
                    mousePosition.z = zOffset;

                    offset = transform.position - mainCamera.ScreenToWorldPoint(mousePosition);

                    // !! Debug
                    Debug.Log("Started dragging " + gameObject.name);
                }
            }
        }

        // Drag 
        if (isDragging && mouse.leftButton.isPressed)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            mousePosition.z = zOffset;

            Vector3 moveTo = mainCamera.ScreenToWorldPoint(mousePosition) + offset;
            transform.position = moveTo;
        }

        // Release
        if (isDragging && mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;

            transform.position = originalPosition;
            moveScript.SetStatus(true);

            // !! Debug
            Debug.Log("Stopped dragging " + gameObject.name);
        }
    }
}
