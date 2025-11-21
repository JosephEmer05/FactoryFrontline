using UnityEngine;

public class MoveAlongConveyor : MonoBehaviour
{
    [Header("Component Config")]
    [SerializeField] private float moveSpeed = 2f;
    // Distance between each component in the conveyor
    [SerializeField] private float checkDistance = 3f;

    private Transform endPoint;
    private LayerMask itemLayer;

    private bool isMoving;

    void Update()
    {
        if (IsItemInFront(out Vector3 hitPoint))
        {
            Vector3 direction = (endPoint.position - transform.position).normalized;
            Vector3 targetPos = hitPoint - direction * checkDistance;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            //isMoving = false;
            return;
        }

        /*if (ReachedEnd())
        {
            transform.position = endPoint.position;
            isMoving = false;
            return;
        }*/

        MoveTowardsEnd();
    }

    void MoveTowardsEnd()
    {
        if (ReachedEnd())
        {
            // Snap to endpoint ONCE it reaches it
            transform.position = endPoint.position;
        }

        //Vector3 direction = (endPoint.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, moveSpeed * Time.deltaTime);
        /*if (Vector3.Distance(transform.position, endPoint.position) < 0.05f)
        {
            transform.position = endPoint.position;
            isMoving = false;
        }*/
    }

    bool ReachedEnd()
    {
        return Vector3.Distance(transform.position, endPoint.position) <= 0.01f;
    }

    bool IsItemInFront(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        Vector3 direction = (endPoint.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, checkDistance + 0.2f, itemLayer))
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

    // Setters
    public void SetEndPoint(Transform point)
    {
        endPoint = point;
    }
    public void SetStatus(bool moving)
    {
        isMoving = moving;
    }
}
