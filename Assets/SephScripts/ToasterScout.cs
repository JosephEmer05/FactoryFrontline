using UnityEngine;
using System.Collections;

public class ToasterScout : BaseEnemy
{
    [Header("Toaster Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireCooldown = 2f;

    private float fireTimer = 0f;

    protected override void Update()
    {
        base.Update();

        fireTimer -= Time.deltaTime;

        Collider[] towersInRange = Physics.OverlapSphere(transform.position, atkRange, towerLayer);
        if (towersInRange.Length > 0)
        {
            Transform targetTower = towersInRange[0].transform;
            FaceTarget(targetTower.position);

            if (fireTimer <= 0f)
            {
                ShootProjectile(targetTower);
                fireTimer = fireCooldown;
            }
        }
    }

    void ShootProjectile(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 dir = (target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, rot);

        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.speed = projectileSpeed;
            proj.SetTarget(target);
        }
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }
}
