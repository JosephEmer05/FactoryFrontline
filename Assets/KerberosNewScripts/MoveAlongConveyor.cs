using UnityEngine;

public class MoveAlongConveyor : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float moveSpeed = 2f;
    public Transform endPoint;
    public LayerMask itemLayer;
    // Distance between each component in the conveyor
    public float checkDistance = 3f;

    private bool isMoving = true;

    /*void Awake()
    {
        if (endPoint != null)
        {
            moveTo = (endPoint.position - transform.position).normalized;
        }
        else
        {
            moveTo = Vector3.right;
        }
    }*/

    void Update()
    {
        if (IsItemInFront(out Vector3 hitPoint))
        {
            Vector3 direction = (endPoint.position - transform.position).normalized;
            transform.position = hitPoint - direction * checkDistance;
            isMoving = false;
            return;
        }

        if (ReachedEnd())
        {
            transform.position = endPoint.position;
            isMoving = false;
            return;
        }

        MoveTowardsEnd();
    }

    void MoveTowardsEnd()
    {
        if (endPoint == null)
        {
            Debug.LogWarning("MoveItem missing End Point reference!");
            return;
        }

        Vector3 direction = (endPoint.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, endPoint.position) < 0.05f)
        {
            transform.position = endPoint.position;
            isMoving = false;
        }
    }

    bool ReachedEnd()
    {
        if (endPoint == null) return false;

        return Vector3.Distance(transform.position, endPoint.position) <= 0.01f;
    }

    bool IsItemInFront(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;

        if (endPoint == null) return false;

        Vector3 direction = (endPoint.position - transform.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, checkDistance + 0.2f, itemLayer))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                hitPoint = hit.collider.transform.position;
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * checkDistance);
    }

    public void SetStatus(bool moving)
    {
        isMoving = moving;
    }
}
