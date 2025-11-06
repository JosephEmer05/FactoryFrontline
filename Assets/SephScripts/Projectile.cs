using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 5f;
    public float lifetime = 5f;
    public bool useTrigger = true;
    public bool homing = false;
    public float homingTurnSpeed = 360f;

    Rigidbody rb;
    Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            if (rb != null && rb.linearVelocity.magnitude == 0f)
                rb.linearVelocity = transform.forward * speed;
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;

        if (homing)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation, targetRot, homingTurnSpeed * Time.fixedDeltaTime);
            transform.rotation = newRot;

            if (rb != null)
                rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            if (rb != null)
                rb.linearVelocity = dir * speed;
        }
    }

    public void SetTarget(Transform t, bool immediateFace = true)
    {
        target = t;
        if (target != null && immediateFace)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
            if (rb != null && !homing)
                rb.linearVelocity = dir * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy e = other.GetComponent<BaseEnemy>();
            if (e != null)
                e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (useTrigger) return;
        if (col.collider.CompareTag("Enemy"))
        {
            BaseEnemy e = col.collider.GetComponent<BaseEnemy>();
            if (e != null)
                e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
