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

    void Update()
    {
        if (isAttacking || waypoints == null || waypoints.Length == 0) return;

        MoveAlongPath();
        DetectTargets();
    }

    public void AssignPath(Transform[] path)
    {
        waypoints = path;
        Index = 0;
    }

    void MoveAlongPath()
    {
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

    void DetectTargets()
    {
        Collider[] towerHits = Physics.OverlapSphere(transform.position, atkRange, towerLayer);
        if (towerHits.Length > 0)
        {
            targetTower = towerHits[0].transform;
            StartCoroutine(AttackTower());
            return;
        }

        Collider[] baseHits = Physics.OverlapSphere(transform.position, atkRange, baseLayer);
        if (baseHits.Length > 0)
        {
            targetBase = baseHits[0].transform;
            StartCoroutine(AttackBase());
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
            StartCoroutine(AttackBase());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
