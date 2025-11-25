using System.Collections;
using UnityEngine;

public class HighFlyDrone : BaseEnemy
{
    [Header("Drone Settings")]
    public float scanRange = 8f;
    public float retreatHeight = 3f;
    public float retreatDistance = 3f;
    public float retreatSpeed = 5f;

    private bool isEngaging = false;
    private Vector3 originalPosition;

    protected override void Update()
    {
        if (!isEngaging && !isAttacking) // avoid moving while base attack happening
        {
            base.Update();
            ScanForTower();
        }
    }

    void ScanForTower()
    {
        // Prioritize base if inside range
        Transform baseInRange = DetectBaseInRange();
        if (baseInRange != null)
        {
            targetBase = baseInRange;
            StartCoroutine(AttackBase());
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, scanRange, towerLayer);
        if (hits.Length > 0)
        {
            Transform tower = hits[0].transform;
            StartCoroutine(EngageTower(tower));
        }
    }

    IEnumerator EngageTower(Transform tower)
    {
        isEngaging = true;
        originalPosition = transform.position;

        while (tower != null)
        {
            // If base appears during engagement, switch
            Transform baseInRange = DetectBaseInRange();
            if (baseInRange != null)
            {
                targetBase = baseInRange;
                StartCoroutine(AttackBase());
                break;
            }

            while (tower != null && Vector3.Distance(transform.position, tower.position) > atkRange)
            {
                transform.position = Vector3.MoveTowards(transform.position, tower.position, speed * Time.deltaTime);
                Vector3 dir = (tower.position - transform.position).normalized;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(dir);
                yield return null;

                // Poll base during approach
                baseInRange = DetectBaseInRange();
                if (baseInRange != null)
                {
                    targetBase = baseInRange;
                    StartCoroutine(AttackBase());
                    tower = null;
                    break;
                }
            }

            if (tower == null) break;

            TestTower towerComp = tower.GetComponent<TestTower>();
            if (towerComp != null)
            {
                towerComp.TakeDamage(atkDmg);
                yield return new WaitForSeconds(atkCooldown);

                if (towerComp.health <= 0)
                {
                    break;
                }
            }
            else
            {
                break;
            }

            Vector3 retreatPoint = transform.position + (Vector3.up * retreatHeight) + (Vector3.right * retreatDistance);
            float retreatTimer = 0f;
            while (Vector3.Distance(transform.position, retreatPoint) > 0.1f && retreatTimer < 2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, retreatPoint, retreatSpeed * Time.deltaTime);
                retreatTimer += Time.deltaTime;
                yield return null;

                // Poll base during retreat
                Transform baseInRangeRetreat = DetectBaseInRange();
                if (baseInRangeRetreat != null)
                {
                    targetBase = baseInRangeRetreat;
                    StartCoroutine(AttackBase());
                    tower = null;
                    break;
                }
            }
        }

        isEngaging = false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }
}
