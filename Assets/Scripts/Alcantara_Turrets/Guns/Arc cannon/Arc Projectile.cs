using System.Collections.Generic;
using UnityEngine;

public class Arc_Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int maxJumps = 3;
    public float hitCooldown = 0.05f;
    public float maxRange = 10f;
    public float damage = 5f;
    private Transform target;
    private int jumps = 0;
    private HashSet<int> hitIds = new HashSet<int>();
    private float ignoreCollisionUntil = 0f;
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        // Destroy if projectile moves beyond max range from spawn point
        if (Vector3.Distance(spawnPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
            return;
        }

        // If no target, find a new one
        if (target == null)
        {
            target = FindNewTarget(hitIds);
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Move directly toward the target's current position
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Face the direction of travel
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    Transform FindNewTarget(HashSet<int> excludeIds)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            if (excludeIds.Contains(enemy.GetInstanceID())) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            float distanceFromSpawn = Vector3.Distance(spawnPosition, enemy.transform.position);


            if (distanceFromSpawn > maxRange) continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < ignoreCollisionUntil) return;
        if (!other.CompareTag("Enemy")) return;

        GameObject hitEnemy = other.gameObject;
        int hitId = hitEnemy.GetInstanceID();

        // Mark enemy as already hit
        hitIds.Add(hitId);
        jumps++;

        // APPLY DAMAGE INSTEAD OF DELETING
        BaseEnemy health = hitEnemy.GetComponent<BaseEnemy>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Find next target (excluding all previously hit)
        Transform nextTarget = FindNewTarget(hitIds);

        if (jumps >= maxJumps || nextTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        target = nextTarget;

        // Small push to prevent instant retrigger
        transform.position += transform.forward * 0.1f;
        ignoreCollisionUntil = Time.time + hitCooldown;
    }

}
