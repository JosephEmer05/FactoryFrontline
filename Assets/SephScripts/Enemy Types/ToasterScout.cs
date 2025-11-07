using System.Collections;
using UnityEngine;

public class ToasterScout : BaseEnemy
{
    [Header("Toaster Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireCooldown = 2f;
    public float engageRangeMultiplier = 1.5f;

    private float cooldownTimer = 0f;

    protected override void Update()
    {
        base.Update();

        cooldownTimer -= Time.deltaTime;

        if (!isAttacking)
        {
            Transform towerTarget = FindClosestTower();
            if (towerTarget != null && Vector3.Distance(transform.position, towerTarget.position) <= atkRange * engageRangeMultiplier)
            {
                StartCoroutine(FireAtTower(towerTarget));
            }
        }
    }

    IEnumerator FireAtTower(Transform target)
    {
        isAttacking = true;

        while (target != null && Vector3.Distance(transform.position, target.position) <= atkRange * engageRangeMultiplier)
        {
            FaceTarget(target.position);

            if (cooldownTimer <= 0f)
            {
                FireProjectile(target);
                cooldownTimer = fireCooldown;
            }

            yield return null;
        }

        isAttacking = false;
        yield return null;
    }

    void FireProjectile(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (target.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        StartCoroutine(MoveProjectile(projectile, direction, target));
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 direction, Transform target)
    {
        float lifetime = 5f;
        float elapsed = 0f;

        while (projectile != null && elapsed < lifetime)
        {
            if (projectile == null) yield break;
            if (target == null)
            {
                Destroy(projectile);
                yield break;
            }

            projectile.transform.Translate(direction * projectileSpeed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;

            if (target != null && Vector3.Distance(projectile.transform.position, target.position) <= 0.5f)
            {
                TestTower tower = target.GetComponent<TestTower>();
                if (tower != null)
                {
                    tower.TakeDamage(atkDmg);
                }

                Destroy(projectile);
                yield break;
            }

            yield return null;
        }

        if (projectile != null)
            Destroy(projectile);
    }


    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }

}
