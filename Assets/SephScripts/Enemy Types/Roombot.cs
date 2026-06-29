using System.Collections;
using UnityEngine;

public class Roombot : BaseEnemy
{
    [Header("Scan Settings")]
    public float scanRange = 8f;

    void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                // Base priority
                Transform baseTransform = DetectBaseInRange();
                if (baseTransform != null)
                {
                    targetBase = baseTransform;
                    StartCoroutine(AttackBase());
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                Transform tower = FindTowerPhysics(scanRange);
                if (tower != null)
                {
                    targetTower = tower;
                    StartCoroutine(AttackTower());
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    Transform FindTowerPhysics(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, towerLayer);
        if (hits.Length > 0)
            return hits[0].transform;
        return null;
    }

    IEnumerator AttackTower()
    {
        isAttacking = true;

        while (true)
        {
            // Switch to base if appears
            Transform baseInRange = DetectBaseInRange();
            if (baseInRange != null)
            {
                targetBase = baseInRange;
                StartCoroutine(AttackBase());
                break;
            }

            if (targetTower == null)
            {
                targetTower = FindTowerPhysics(scanRange);
                if (targetTower == null)
                    break;
            }

            float dist = Vector3.Distance(transform.position, targetTower.position);
            if (dist > atkRange)
                break;

            TestTower towerComp = targetTower.GetComponent<TestTower>();
            if (towerComp == null)
            {
                targetTower = null;
                continue;
            }

            towerComp.TakeDamage(atkDmg);

            if (towerComp.health <= 0)
                break;

            yield return new WaitForSeconds(atkCooldown);
        }

        isAttacking = false;
        targetTower = null;
    }
}
