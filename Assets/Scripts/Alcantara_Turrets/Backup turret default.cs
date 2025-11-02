//using UnityEngine;
//using System.Linq;

//public class Backup_Turret_Aiming : MonoBehaviour
//{
//    public GameObject projectilePrefab;  // Normal projectile
//    public float range = 5f;
//    public float cooldown = 2f;
//    public float projectileSpeed = 15f;
//    private float lastFireTime = 0f;
//    private Transform currentTarget;

//    void Update()
//    {
//        currentTarget = FindClosestEnemy();

//        if (currentTarget != null)
//        {
//            // Rotate turret toward target (optional)
//            Vector3 direction = currentTarget.position - transform.position;
//            if (direction != Vector3.zero)
//                transform.rotation = Quaternion.LookRotation(direction);

//            if (Time.time >= lastFireTime + cooldown)
//            {
//                FireBullet(currentTarget);
//                lastFireTime = Time.time;
//            }
//        }
//    }

//    Transform FindClosestEnemy()
//    {
//        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
//        return enemies
//            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= range)
//            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
//            .Select(e => e.transform)
//            .FirstOrDefault();
//    }

//    void FireBullet(Transform target)
//    {
//        // Fire from turret's body center instead of firePoint
//        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

//        // Make it face the target
//        Vector3 direction = (target.position - transform.position).normalized;
//        bullet.transform.rotation = Quaternion.LookRotation(direction);

//        // Launch it toward the target
//        Rigidbody rb = bullet.GetComponent<Rigidbody>();
//        if (rb != null)
//            rb.linearVelocity = direction * projectileSpeed;

//        // Optional: tell projectile its target (if it has seeking logic)
//        Projectile proj = bullet.GetComponent<Projectile>();
//        if (proj != null)
//            proj.SetTarget(target);
//    }
//}
