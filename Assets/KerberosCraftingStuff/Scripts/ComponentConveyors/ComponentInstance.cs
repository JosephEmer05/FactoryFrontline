using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ComponentInstance : MonoBehaviour
{
    public ComponentData componentData;
    public ComponentConveyorManager conveyor;
    private Camera mainCamera;

    [Header("Dragging Config")]
    public bool isDragging { get; set; }
    public bool inSlot = false;
    public Vector3 originalPos { get; set; }

    private float zOffset;
    private Vector3 offset;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        conveyor.AddComponent(this);
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

                    // !! Debug
                    //Debug.Log("Started dragging " + gameObject.name);
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

            // Check for closest 3D slot
            UICraftingSlot slot = FindClosestSlot();

            if (slot != null)
            {
                PlaceIntoSlot(slot);
            }
            else
            {
                transform.position = originalPos;
            }

            // !! Debug
            //Debug.Log("Stopped dragging " + gameObject.name);
        }
    }

    UICraftingSlot FindClosestSlot()
    {
        float closestDist = float.MaxValue;
        UICraftingSlot closestSlot = null;

        foreach (var slot in CraftSlotManager.Slots)
        {
            if (slot.slotType != componentData.type)
            {
                continue;
            }

            float dist = Vector3.Distance(transform.position, slot.WorldPosition);

            if (dist < closestDist && dist < 2f)
            {
                closestDist = dist;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }

    void PlaceIntoSlot(UICraftingSlot slot)
    {
        // Check if the component matches the slot's type
        if (componentData.type != slot.slotType)
        {
            return;
        }
            
        inSlot = true;
        conveyor.RemoveComponent(this);

        // Snap into slot
        transform.position = slot.transform.position;

        slot.SetPart(componentData, this.gameObject);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
