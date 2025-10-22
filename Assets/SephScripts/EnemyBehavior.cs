using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 2f;

    [Header("Attack Settings")]
    public float atkRange = 1f;
    public float atkDmg = 3f;
    public float atkCooldown = 1f;
    public LayerMask towerLayer;
    public LayerMask baseLayer;

    private int Index = 0;
    private bool isAttacking = false;
    private Transform targetTower;
    private Transform targetBase;
    private Coroutine attackRoutine;

    void Start()
    {
        StartCoroutine(ScanForTowersRoutine());
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!isAttacking)
        {
            MoveAlongPath();
        }
        else
        {
            if (targetTower != null)
            {
                Vector3 dir = (targetTower.position - transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                }
            }
        }
    }

    public void AssignPath(Transform[] path)
    {
        waypoints = path;
        Index = 0;
    }

    void MoveAlongPath()
    {
        if (Index >= waypoints.Length) return;

        Transform target = waypoints[Index];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Index++;
            if (Index >= waypoints.Length)
            {
                ReachBase();
            }
        }
    }

    IEnumerator ScanForTowersRoutine()
    {
        while (true)
        {
            if (!isAttacking)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, atkRange, towerLayer);

                if (hits.Length > 0)
                {
                    float closestDist = Mathf.Infinity;
                    Transform closestTower = null;

                    foreach (Collider hit in hits)
                    {
                        float dist = Vector3.Distance(transform.position, hit.transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestTower = hit.transform;
                        }
                    }

                    if (closestTower != null && !isAttacking)
                    {
                        targetTower = closestTower;
                        attackRoutine = StartCoroutine(AttackTower());
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator AttackTower()
    {
        isAttacking = true;

        while (targetTower != null && Vector3.Distance(transform.position, targetTower.position) <= atkRange)
        {
            TestTower tower = targetTower.GetComponent<TestTower>();
            if (tower != null)
            {
                tower.TakeDamage(atkDmg);
                yield return new WaitForSeconds(atkCooldown);

                if (tower.health <= 0)
                {
                    targetTower = null;
                    break;
                }
            }
            else
            {
                targetTower = null;
                break;
            }
        }

        isAttacking = false;
        attackRoutine = null;
        targetTower = null;

        yield return null;
    }

    IEnumerator AttackBase()
    {
        isAttacking = true;

        while (targetBase != null && Vector3.Distance(transform.position, targetBase.position) <= atkRange)
        {
            TestBase baseTarget = targetBase.GetComponent<TestBase>();
            if (baseTarget != null)
            {
                baseTarget.TakeDamage(atkDmg);
                yield return new WaitForSeconds(atkCooldown);

                if (baseTarget.health <= 0)
                {
                    targetBase = null;
                    break;
                }
            }
            else
            {
                targetBase = null;
                break;
            }
        }

        isAttacking = false;
        if (targetBase == null)
            Destroy(gameObject);
    }

    void ReachBase()
    {
        Collider[] baseHits = Physics.OverlapSphere(transform.position, atkRange, baseLayer);
        if (baseHits.Length > 0)
        {
            targetBase = baseHits[0].transform;
            if (!isAttacking)
            {
                StartCoroutine(AttackBase());
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
