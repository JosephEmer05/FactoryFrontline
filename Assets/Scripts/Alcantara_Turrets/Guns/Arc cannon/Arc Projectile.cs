using System.Collections.Generic;
using UnityEngine;

public class Arc_Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int maxJumps = 3;
    public float hitCooldown = 0.05f;

    private Transform target;
    private int jumps = 0;
    private HashSet<int> hitIds = new HashSet<int>();
    private float ignoreCollisionUntil = 0f;

    void Update()
    {
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

        // Mark as hit
        hitIds.Add(hitId);
        jumps++;

        // Find next target BEFORE destroying this one
        Transform nextTarget = FindNewTarget(hitIds);

        // Destroy the hit enemy
        Destroy(hitEnemy);

        if (jumps >= maxJumps || nextTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        // Immediately retarget and continue homing
        target = nextTarget;

        // Move slightly forward and delay re-collision
        transform.position += transform.forward * 0.1f;
        ignoreCollisionUntil = Time.time + hitCooldown;
    }
}
