using UnityEngine;

public class TowerConveyorMover : MonoBehaviour
{
    private TowerOutputConveyor conveyor;
    private Vector3 endPosition;
    private float moveSpeed;
    private float spacing;
    private LayerMask towerLayer;
    private bool canMove = true;

    public void Initialize(TowerOutputConveyor conveyorRef, Vector3 endPos, float speed, float space, LayerMask layer)
    {
        conveyor = conveyorRef;
        endPosition = endPos;
        moveSpeed = speed;
        spacing = space;
        towerLayer = layer;
    }

    void Update()
    {
        if (canMove)
        {
            if (IsTowerAhead())
            {
                canMove = false;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, endPosition) < 0.1f)
            {
                canMove = false;
                conveyor.RemoveTower(gameObject);
            }
        }
        else
        {
            if (!IsTowerAhead())
                canMove = true;
        }
    }

    bool IsTowerAhead()
    {
        RaycastHit hit;
        Vector3 direction = Vector3.right;
        Vector3 origin = transform.position + Vector3.right * 0.5f;

        if (Physics.Raycast(origin, direction, out hit, spacing, towerLayer))
        {
            return true;
        }

        return false;
    }
}
