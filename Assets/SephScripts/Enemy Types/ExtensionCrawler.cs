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
            base.Update();
            ScanForTargets();
        }
    }

    void ScanForTargets()
    {
        // Base priority
        Transform baseInRange = DetectBaseInRange();
        if (baseInRange != null)
        {
            targetBase = baseInRange;
            StartCoroutine(AttackBase());
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attachRange, towerLayer);
        if (hits.Length > 0)
        {
            Transform target = hits[0].transform;
            StartCoroutine(WrapTower(target));
        }
    }

    IEnumerator WrapTower(Transform targetTower)
    {
        isWrapping = true;

        Vector3 latchPoint = targetTower.position + (transform.position - targetTower.position).normalized * 0.5f;

        while (Vector3.Distance(transform.position, latchPoint) > 0.1f)
        {
            // Switch to base if appears
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
        TestTower tower = targetTower.GetComponent<TestTower>();

        while (tower != null && timer < latchDuration)
        {
            // Poll base while latched
            Transform baseInRange = DetectBaseInRange();
            if (baseInRange != null)
            {
                targetBase = baseInRange;
                StartCoroutine(AttackBase());
                isWrapping = false;
                yield break;
            }

            tower.TakeDamage(wrapDamage);
            yield return new WaitForSeconds(damageInterval);
            timer += damageInterval;

            if (tower.health <= 0) break;
        }

        isWrapping = false;
        Destroy(gameObject);
    }
}
