using UnityEngine;

public class Spread_Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 1.5f;
    public bool homing = false;
    public Alltowerscript owner; // turret providing damage
    private Transform target;

    void Start()
    {
        // ensure projectile despawns after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (homing && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            // Face the direction of movement
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            // Move forward along initial facing direction (no homing)
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    public void SetTarget(Transform t) { target = t; }

    private void ApplyHit(BaseEnemy enemy)
    {
        if (enemy != null && owner != null)
            enemy.TakeDamage(owner.CurrentDamage);
        Destroy(gameObject); // pellet consumed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        ApplyHit(other.GetComponent<BaseEnemy>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Enemy")) return;
        ApplyHit(collision.collider.GetComponent<BaseEnemy>());
    }
}
