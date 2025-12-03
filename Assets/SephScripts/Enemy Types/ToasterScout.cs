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

    // Tiny radius used only for base detection so they look like they crash into the base
    private const float BaseContactRadius = 0.2f;

    private float cooldownTimer = 0f;

    protected override void Update()
    {
        base.Update();

        cooldownTimer -= Time.deltaTime;

        if (!isAttacking)
        {
            // Prioritize base: require near-contact distance so it looks like a crash
            Transform baseNearContact = DetectBaseNearContact();
            if (baseNearContact != null)
            {
                targetBase = baseNearContact;
                // Single-hit damage and destroy
                DealDamage(targetBase, 1f);
                Die();
                return;
            }

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
            // Switch if base appears: only when near contact
            Transform baseNearContact = DetectBaseNearContact();
            if (baseNearContact != null)
            {
                targetBase = baseNearContact;
                DealDamage(targetBase, 1f);
                Die();
                break;
            }

            FaceTarget(target.position);

            if (cooldownTimer <= 0f)
            {
                FireProjectileAtTower(target);
                cooldownTimer = fireCooldown;
            }

            yield return null;
        }

        isAttacking = false;
        yield return null;
    }

    // Restrict base detection to a tiny radius around the enemy
    private Transform DetectBaseNearContact()
    {
        // Use physics overlap with baseLayer and a very small radius
        Collider[] hits = Physics.OverlapSphere(transform.position, BaseContactRadius, baseLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            // Return the first valid base transform
            return hits[i].transform;
        }
        return null;
    }

    void FireProjectileAtTower(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (target.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        StartCoroutine(MoveProjectileTower(projectile, direction, target));
    }

    IEnumerator MoveProjectileTower(GameObject projectile, Vector3 direction, Transform target)
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

    // Keep ReachBase empty; near-contact logic handles damage and destruction.
    protected override void ReachBase()
    {
        // Intentionally empty.
    }
}
