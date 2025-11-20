using UnityEngine;
using UnityEngine.InputSystem;

public class ComponentInstance : MonoBehaviour
{
    private Camera cam;
    private MoveAlongConveyor move;
    private ItemSpawner spawner;

    private bool isDragging = false;
    private float zOffset;
    private Vector3 grabOffset;
    private Vector3 originalPos;

    public ComponentData componentData;

    void Awake()
    {
        cam = Camera.main;
        move = GetComponent<MoveAlongConveyor>();
        spawner = move.ownerSpawner;
    }

    void Update()
    {
        var mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                originalPos = transform.position;

                if (spawner != null)
                    spawner.PauseAll();

                zOffset = cam.WorldToScreenPoint(transform.position).z;
                Vector3 mp = mouse.position.ReadValue();
                mp.z = zOffset;

                grabOffset = transform.position - cam.ScreenToWorldPoint(mp);
            }
        }

        if (isDragging)
        {
            Vector3 mp = mouse.position.ReadValue();
            mp.z = zOffset;
            transform.position = cam.ScreenToWorldPoint(mp) + grabOffset;
        }

        if (isDragging && mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            transform.position = originalPos;
            move.Resume();

            if (spawner != null)
                spawner.ResumeAll();
        }
    }
}
