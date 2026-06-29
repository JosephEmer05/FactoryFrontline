using UnityEngine;
using System.Linq;

public class Single_turret : MonoBehaviour
{
    public GameObject projectilePrefab;  // Normal projectile
    public float cooldown = 0f;
    public float projectileSpeed = 15f;
    private float lastFireTime = 0f;
    private Transform currentTarget;
    public float spawnOffset = 1f; // distance in front of turret to spawn projectile

    void Start()
    {
        if (projectilePrefab == null)
        {
            projectilePrefab = Resources.Load<GameObject>("Prefabs/SinglePrefab");
            if (projectilePrefab == null)
            {
                Debug.LogError("Single_Turret: Could not find 'SinglePrefab' in a 'Resources/Prefabs' folder. " +
                                   "Please either assign it in the Inspector or move it to 'Assets/Resources/Prefabs/SinglePrefab.prefab'.");
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
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void FireBullet(Transform target)
    {
        // Compute spawn position slightly in front of turret to avoid overlap
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + direction * spawnOffset;

        GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Make it face the target
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        // Prevent immediate collision with any object tagged "Turret"
        Collider bulletCol = bullet.GetComponent<Collider>();
        if (bulletCol != null)
        {
            // Ignore collisions with all colliders on objects tagged "Turret"
            GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
            foreach (GameObject t in turrets)
            {
                Collider[] turretCols = t.GetComponentsInChildren<Collider>();
                foreach (Collider tc in turretCols)
                    Physics.IgnoreCollision(bulletCol, tc, true);
            }

            // Also ignore with this turret's colliders (defensive)
            Collider myCol = GetComponent<Collider>();
            if (myCol != null)
                Physics.IgnoreCollision(bulletCol, myCol, true);
        }

        // Launch it toward the target
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed; // use velocity

        // Optional: tell projectile its target (if it has seeking logic)
        Single_Projectile proj = bullet.GetComponent<Single_Projectile>();
        if (proj != null)
            proj.SetTarget(target);
    }
}
