using UnityEngine;
using System.Linq;

public class Turret_Aiming : MonoBehaviour
{
    public GameObject projectilePrefab; // Normal projectile
    public Transform firePoint;
    public float range = 5f;
    public float cooldown = 2f;
    public float projectileSpeed = 5f;
    public float rotationSpeed = 5f;
    private float lastFireTime = 0f;
    private Transform currentTarget;

    void Update()
    {
        currentTarget = FindClosestEnemy();

        if (currentTarget != null)
        {
            RotateTowardsTarget(currentTarget.position);

            if (Time.time >= lastFireTime + cooldown)
            {
                FireProjectile(currentTarget);

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

    void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void FireProjectile(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection((target.position - firePoint.position).normalized);
    }
}
