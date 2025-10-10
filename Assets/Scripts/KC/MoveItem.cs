using UnityEngine;

public class MoveItem : MonoBehaviour
{
    public float speed = 2f;
    public float stopY = 5f;           //Max Y position to stop at
    public float checkDistance = 1.1f; //Distance to check for stacked items above
    public LayerMask itemLayer;        //Layer assigned to items

    private bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            //Only move if not below another item
            if (!IsStackedBelow())
            {
                transform.Translate(Vector3.up * speed * Time.deltaTime);

                //Clamp position if it reaches the top
                if (transform.position.y >= stopY)
                {
                    Vector3 pos = transform.position;
                    pos.y = stopY;
                    transform.position = pos;
                    isMoving = false; // Stop when we reach top
                }
            }
        }
    }

    bool IsStackedBelow()
    {
        //Checks if there's an item above
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, Vector3.up, out hit, checkDistance, itemLayer))
        {
            return true; //There's an item above
        }

        return false;
    }

    public void PickUp()
    {
        Debug.Log("Picked up item: " + gameObject.name);
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        PickUp();
    }
}
