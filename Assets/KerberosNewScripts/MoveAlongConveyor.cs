using UnityEngine;

public class MoveAlongConveyor : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float moveSpeed = 2f;
    public Transform endPoint;
    public LayerMask itemLayer;
    public float stopDistance = 1f;

    [HideInInspector] public ItemSpawner ownerSpawner;

    private bool isPaused = false;

    void Update()
    {
        if (isPaused) return; // 🚫 Full stop

        if (IsItemInFront()) return;

        MoveForward();
    }

    public void Pause() => isPaused = true;

    public void Resume() => isPaused = false;

    void MoveForward()
    {
        if (endPoint == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );
    }

    bool IsItemInFront()
    {
        if (endPoint == null) return false;

        Vector3 dir = (endPoint.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, stopDistance, itemLayer))
        {
            if (hit.collider.gameObject != gameObject)
                return true;
        }

        return false;
    }
}
