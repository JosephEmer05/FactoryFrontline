using System.Collections;
using UnityEngine;

public class ExtensionCrawler : BaseEnemy
{
    [Header("Crawler Settings")]
    public float attachRange = 1.5f;
    public float wrapDamage = 2f;
    public float damageInterval = 0.5f;
    public float latchDuration = 3f;

    private bool isWrapping = false;

    protected override void Update()
    {
        if (!isWrapping && !isAttacking)
        {
            // Continue normal movement toward base
            base.Update();
            ScanForTargets();
        }
    }

    void ScanForTargets()
    {
        // Prefer base if in range
        Transform baseInRange = DetectBaseInRange();
        if (baseInRange != null)
        {
            targetBase = baseInRange;
            StartCoroutine(AttackBase());
            return;
        }

        // Look for closest tower within attach range
        Transform tower = FindClosestTower();
        if (tower != null && Vector3.Distance(transform.position, tower.position) <= attachRange)
        {
            StartCoroutine(WrapTower(tower));
        }
    }

    IEnumerator WrapTower(Transform targetTower)
    {
        if (targetTower == null) yield break;
        isWrapping = true;
        targetTower = targetTower; // local reference

        // Compute latch point offset slightly toward crawler start position
        Vector3 latchPoint = targetTower.position + (transform.position - targetTower.position).normalized * 0.5f;

        // Approach latch point unless a base becomes available
        while (targetTower != null && Vector3.Distance(transform.position, latchPoint) > 0.1f)
        {
            Transform baseInRange = DetectBaseInRange();
            if (baseInRange != null)
            {
                targetBase = baseInRange;
                StartCoroutine(AttackBase());
                isWrapping = false;
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, latchPoint, speed * Time.deltaTime);
            yield return null;
        }

        float timer = 0f;
        TestTower towerComponent = targetTower ? targetTower.GetComponent<TestTower>() : null;

        // Damage loop (wrap duration or tower death or base priority)
        while (towerComponent != null && timer < latchDuration && towerComponent.health > 0f)
        {
            Transform baseInRange = DetectBaseInRange();
            if (baseInRange != null)
            {
                targetBase = baseInRange;
                StartCoroutine(AttackBase());
                isWrapping = false;
                yield break;
            }

            towerComponent.TakeDamage(wrapDamage);
            yield return new WaitForSeconds(damageInterval);
            timer += damageInterval;
        }

        // If tower is destroyed, continue normal behavior then repeat
        isWrapping = false;
        targetTower = null;
        // Movement & scanning will resume in Update()
    }
}
