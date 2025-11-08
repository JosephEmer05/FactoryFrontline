using UnityEngine;
using System.Linq;

public class Spread_turret : MonoBehaviour
{
    public GameObject spreadPrefab;  // Normal projectile
    public float range = 5f;
    public float cooldown = 1f;
    public float projectileSpeed = 15f;
    private float lastFireTime = 0f;
    private Transform currentTarget;

    [Tooltip("Number of pellets per shot")]
    public int pelletCount = 8;

    [Tooltip("Total cone angle in degrees")]
    public float spreadAngle = 20f;

        void start()
    {
        if (projectilePrefab == null)
        {
            projectilePrefab = Resources.Load<GameObject>("Prefabs/SpreadPrefab");
            if (projectilePrefab == null)
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
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void FireBullet(Transform target)
    {
        // Base direction toward the target
        Vector3 baseDirection = (target.position - transform.position).normalized;

        // Spawn multiple pellets in the cone
        for (int i = 0; i < Mathf.Max(1, pelletCount); i++)
        {
            // Random yaw and pitch within the spread cone
            float halfAngle = spreadAngle * 0.5f;
            float randomYaw = Random.Range(-halfAngle, halfAngle);
            float randomPitch = Random.Range(-halfAngle, halfAngle);

            Quaternion yawRot = Quaternion.AngleAxis(randomYaw, Vector3.up);
            Quaternion pitchRot = Quaternion.AngleAxis(randomPitch, transform.right);

            Vector3 pelletDir = yawRot * pitchRot * baseDirection;

            // Instantiate pellet and orient it along its direction
            GameObject pellet = Instantiate(spreadPrefab, transform.position, Quaternion.LookRotation(pelletDir));

            // Launch it toward the pellet direction
            Rigidbody rb = pellet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = pelletDir * projectileSpeed;

            // Optional: tell projectile its target (if it has seeking logic)
            Single_Projectile proj = pellet.GetComponent<Single_Projectile>();
            if (proj != null)
                proj.SetTarget(target);
        }
    }
}
