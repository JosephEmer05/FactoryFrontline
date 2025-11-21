using UnityEngine;

public class Spread_Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 1.5f;

    void Start()
    {
        // ensure projectile despawns after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward along initial facing direction (no homing)
        transform.position += transform.forward * speed * Time.deltaTime;

        // Face the direction of movement
        if (transform.forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    // Utility: walk up parents to find a GameObject with the specified tag
    GameObject FindTaggedParent(GameObject obj, string tag)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            if (t.CompareTag(tag))
                return t.gameObject;
            t = t.parent;
        }
        return null;
    }

    // Handler used by both trigger and collision events
    void HandleHit(GameObject otherObj)
    {
        if (otherObj == null)
            return;

        // Ignore turret objects
        if (otherObj.CompareTag("Turret"))
            return;

        // Try the object itself, then parents
        GameObject enemy = FindTaggedParent(otherObj, "Enemy");
        if (enemy != null)
        {
            Destroy(enemy); // Destroy enemy root
            Destroy(gameObject); // Destroy projectile
        }
    }

    // 3D trigger event
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    // 3D collision event (in case colliders are not triggers)
    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }
}
