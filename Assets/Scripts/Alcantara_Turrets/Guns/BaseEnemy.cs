using System.Collections;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("General Settings")]
    public float health = 20f;
    public float speed = 2f;
    public float atkRange = 1f;
    public float atkDmg = 3f;
    public float atkCooldown = 1f;
    public LayerMask towerLayer;
    public LayerMask baseLayer;

    protected Transform[] waypoints;
    protected int index = 0;
    protected bool isAttacking = false;
    protected Transform targetTower;
    protected Transform targetBase;

    public virtual void AssignPath(Transform[] path)
    {
        waypoints = path;
        index = 0;
    }

    protected virtual void Update()
    {
        if (!isAttacking)
            MoveAlongPath();
    }

    protected void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (index >= waypoints.Length) return;

        Transform target = waypoints[index];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            index++;
            if (index >= waypoints.Length)
                ReachBase();
        }
    }

    protected virtual void ReachBase()
    {
        Collider[] baseHits = Physics.OverlapSphere(transform.position, atkRange, baseLayer);
        if (baseHits.Length > 0)
        {
            targetBase = baseHits[0].transform;
            StartCoroutine(AttackBase());
        }
    }

    protected virtual IEnumerator AttackBase()
    {
        isAttacking = true;
        while (targetBase != null && Vector3.Distance(transform.position, targetBase.position) <= atkRange)
        {
            TestBase baseTarget = targetBase.GetComponent<TestBase>();
            if (baseTarget != null)
                baseTarget.TakeDamage(atkDmg);

            yield return new WaitForSeconds(atkCooldown);
        }

        isAttacking = false;
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected Transform FindClosestTower()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, atkRange, towerLayer);
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}