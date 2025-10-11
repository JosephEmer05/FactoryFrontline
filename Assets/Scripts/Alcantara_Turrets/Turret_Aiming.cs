using UnityEngine;
using System.Linq;

public class Turret_Aiming : MonoBehaviour
{
    public GameObject projectilePrefab; // Normal projectile
    public Transform firePoint;
    public float range = 5f;
    public float cooldown = 2f;
    public float projectileSpeed = 15f;
    private float lastFireTime = 0f;
    private Transform currentTarget;

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
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().SetTarget(target);
    }
}
