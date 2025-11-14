using UnityEngine;
using System.Linq;

public class Spread_turret : MonoBehaviour
{
    public GameObject SpreadPrefab;  // Normal projectile
    public float range = 5f;
    public float cooldown = 1f;
    public float projectileSpeed = 15f;
    private float lastFireTime = 0f;
    private Transform currentTarget;

    [Tooltip("Number of pellets per shot")]
    public int pelletCount = 8;

    [Tooltip("Total cone angle in degrees")]
    public float spreadAngle = 20f;

    [Tooltip("Distance in front of turret to spawn pellets")]
    public float spawnOffset = 1f;

    void Start()
    {
        if (SpreadPrefab == null)
        {
            SpreadPrefab = Resources.Load<GameObject>("Prefabs/SpreadPrefab");
            if (SpreadPrefab == null)
            {
                Debug.LogError("Single_Turret: Could not find 'SpreadPrefab' in a 'Resources/Prefabs' folder. " +
                               "Please either assign it in the Inspector or move it to 'Assets/Resources/Prefabs/SpreadPrefab.prefab'.");
            }
        }
    }
    void Update()
    {
        currentTarget = FindClosestEnemy();

        if (currentTarget != null)
        {
            if (Time.time >= lastFireTime + cooldown)
            {
                FireBullet(currentTarget);
                lastFireTime = Time.time;
            }
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void FireBullet(Transform target)
    {
        // Base direction toward the target (3D)
        Vector3 baseDirection = (target.position - transform.position).normalized;

        // Rotate turret to face the target (visual aiming)
        if (baseDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(baseDirection);

        // Spawn multiple pellets in a 3D cone
        float halfAngle = spreadAngle * 0.5f;
        for (int i = 0; i < Mathf.Max(1, pelletCount); i++)
        {
            // Random yaw and pitch within the spread cone
            float yaw = Random.Range(-halfAngle, halfAngle);
            float pitch = Random.Range(-halfAngle, halfAngle);

            // Build rotation: start with look rotation to base direction, then apply local pitch/yaw offsets
            Quaternion spreadRot = Quaternion.Euler(pitch, yaw, 0f);
            Quaternion pelletRot = Quaternion.LookRotation(baseDirection) * spreadRot;
            Vector3 pelletDir = pelletRot * Vector3.forward;

            // Spawn a bit in front to avoid overlapping the turret collider
            Vector3 spawnPos = transform.position + pelletDir * spawnOffset;

            // Instantiate pellet and orient it along its direction
            GameObject pellet = Instantiate(SpreadPrefab, spawnPos, Quaternion.LookRotation(pelletDir));

            // Launch it toward the pellet direction
            Rigidbody rb = pellet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = pelletDir * projectileSpeed;

            // Optional: tell projectile its target if it supports homing
            Single_Projectile singleProj = pellet.GetComponent<Single_Projectile>();
            if (singleProj != null)
                singleProj.SetTarget(target);
        }
    }
}
