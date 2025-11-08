using UnityEngine;
using System.Linq;

public class Spread_turret : MonoBehaviour
{
    public GameObject projectilePrefab;  // Normal projectile
    public float range = 5f;
    public float cooldown = 1f;
    public float projectileSpeed = 15f;
    private float lastFireTime = 0f;
    private Transform currentTarget;

    [Tooltip("Number of pellets per shot")]
    public int pelletCount = 8;

    [Tooltip("Total cone angle in degrees")]
    public float spreadAngle = 20f;

    void Start()
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
        // Base direction toward the target (in 2D, we only care about x and y)
        Vector3 baseDirection = (target.position - transform.position).normalized;

        // Spawn multiple pellets in a 2D cone
        for (int i = 0; i < Mathf.Max(1, pelletCount); i++)
        {
            // Random angle within the spread cone (only Z-axis rotation for 2D)
            float halfAngle = spreadAngle * 0.5f;
            float randomAngle = Random.Range(-halfAngle, halfAngle);

            // Rotate around Z-axis for 2D spread
            Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
            Vector3 pelletDir = spreadRotation * baseDirection;

            // Instantiate pellet and orient it along its direction
            GameObject pellet = Instantiate(spreadPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, pelletDir));

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
