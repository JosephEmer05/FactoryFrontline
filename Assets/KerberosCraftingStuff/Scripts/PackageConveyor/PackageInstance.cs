using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PackageInstance : MonoBehaviour
{
    // PREFAB OF THE TOWER THE PACKAGE
    // IS MEANT TO SPAWN
    // public GameObject towerPrefab;

    public PackageConveyorManager conveyor;
    private Camera mainCamera;
    
    [Header("Dragging Config")]
    public bool isDragging { get; set; }
    public Vector3 originalPos { get; set; }

    private float zOffset;
    private Vector3 offset;
    
    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        conveyor.AddPackage(this);
    }

    void Update()
    {
        var mouse = Mouse.current;

        // Click
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;

                    zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
                    Vector3 mousePosition = mouse.position.ReadValue();
                    mousePosition.z = zOffset;

                    offset = transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
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

            // <====
            // TOWER PLACEMENT
            // LOGIC GOES HERE
            // <=====

            transform.position = originalPos;
        }
    }
}
