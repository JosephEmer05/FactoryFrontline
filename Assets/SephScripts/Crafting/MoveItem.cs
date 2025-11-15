using UnityEngine;

public class MoveItem : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float moveSpeed = 2f;
    public Transform endPoint;
    public LayerMask itemLayer;
    public float checkDistance = 1.0f;

    private bool isMoving = true;

    void Update()
    {
        if (!isMoving) return;
        if (IsItemInFront())
            return;
        MoveTowardsEnd();
    }

    void MoveTowardsEnd()
    {
        if (endPoint == null)
        {
            Debug.LogWarning("MoveItem missing End Point reference!");
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, endPoint.position) < 0.05f)
        {
            isMoving = false;
        }
    }

    bool IsItemInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right, out hit, checkDistance, itemLayer))
        {
            // Only stop if the detected item is closer to the end than this one
            if (hit.collider != null && hit.collider.gameObject != gameObject)
                return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * checkDistance);
    }
}
