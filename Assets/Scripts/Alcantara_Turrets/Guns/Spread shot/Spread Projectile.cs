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

    // 3D collision event
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // Destroy enemy
            Destroy(gameObject);        // Destroy projectile
        }
    }
}
