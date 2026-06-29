using System.Collections.Generic;
using UnityEngine;

public class Arc_Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int maxJumps = 3;
    public float hitCooldown = 0.05f;
    public float maxRange = 10f;
    public bool homing = true; // if false, travels toward initial target only
    public Alltowerscript owner; // turret providing damage

    private Transform target;
    private int jumps = 0;
    private HashSet<int> hitIds = new HashSet<int>();
    private float ignoreCollisionUntil = 0f;
    private Vector3 spawnPosition;
    private Vector3 initialDirection;

    void Start()
    {
        spawnPosition = transform.position;
        initialDirection = target ? (target.position - transform.position).normalized : transform.forward;
    }

    void Update()
    {
        // Destroy if projectile moves beyond max range from spawn point
        if (Vector3.Distance(spawnPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
            return;
        }

        if (homing)
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
        else
        {
            // Move in the initial direction
            transform.position += initialDirection * speed * Time.deltaTime;
            if (initialDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(initialDirection);
        }
    }

    public void SetTarget(Transform newTarget) { target = newTarget; }

    Transform FindNewTarget(HashSet<int> excludeIds)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null; float minDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            if (excludeIds.Contains(enemy.GetInstanceID())) continue;
            float distSpawn = Vector3.Distance(spawnPosition, enemy.transform.position);
            if (distSpawn > maxRange) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance) { minDistance = distance; closest = enemy.transform; }
        }
        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < ignoreCollisionUntil) return;
        if (!other.CompareTag("Enemy")) return;
        GameObject hitEnemy = other.gameObject; int hitId = hitEnemy.GetInstanceID(); hitIds.Add(hitId); jumps++;
        BaseEnemy health = hitEnemy.GetComponent<BaseEnemy>(); if (health != null && owner != null) health.TakeDamage(owner.CurrentDamage);
        Transform nextTarget = homing ? FindNewTarget(hitIds) : null;
        if (jumps >= maxJumps || nextTarget == null) { Destroy(gameObject); return; }
        target = nextTarget; transform.position += transform.forward * 0.1f; ignoreCollisionUntil = Time.time + hitCooldown;
    }
}
