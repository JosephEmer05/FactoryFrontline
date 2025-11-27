using System.Collections;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public System.Action<BaseEnemy> onEnemyDied;

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

    private bool hasHitBase = false;

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

    // Helper for derived classes to poll for base inside attack range
    protected Transform DetectBaseInRange()
    {
        Collider[] baseHits = Physics.OverlapSphere(transform.position, atkRange, baseLayer);
        if (baseHits.Length > 0)
            return baseHits[0].transform;
        return null;
    }

    // Generic damage helper: tries specific known components then falls back to SendMessage
    protected void DealDamage(Transform target, float damage)
    {
        if (target == null) return;
        var towerComp = target.GetComponent<TestTower>();
        if (towerComp != null)
        {
            towerComp.TakeDamage(damage);
            return;
        }
        // (force 1 damage always)
        var baseComp = target.GetComponent<TestBase>();
        if (baseComp != null)
        {
            baseComp.TakeDamage(1f);
            return;
        }
        // Fallback: any component implementing TakeDamage(float). We cannot know if it's base or tower, so send original amount.
        target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    protected virtual IEnumerator AttackBase()
    {
        isAttacking = true;
        while (targetBase != null)
        {
            float dist = Vector3.Distance(transform.position, targetBase.position);
            if (dist > atkRange)
            {
                // Move closer until within attack range
                transform.position = Vector3.MoveTowards(transform.position, targetBase.position, speed * Time.deltaTime);
                Vector3 dir = (targetBase.position - transform.position).normalized;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                yield return null; // next frame
                continue;
            }

            // Uses DealDamage which enforces 1 damage for base
            DealDamage(targetBase, atkDmg);
            yield return new WaitForSeconds(atkCooldown);
        }

        isAttacking = false;
        Destroy(gameObject);
    }

    // Utility to test if a GameObject is inside a LayerMask
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }

    private void HandleBaseImpact(Transform baseTransform)
    {
        if (hasHitBase) return;
        hasHitBase = true;
        targetBase = baseTransform;
        // Apply a single hit on collision then destroy
        DealDamage(baseTransform, atkDmg);
        Die();
    }

    // Trigger-based collision
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, baseLayer) || other.GetComponent<TestBase>() != null)
        {
            HandleBaseImpact(other.transform);
        }
    }

    // Physics collision
    protected virtual void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;
        if (IsInLayerMask(other.gameObject, baseLayer) || other.GetComponent<TestBase>() != null)
        {
            HandleBaseImpact(other.transform);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    public virtual void Die()
    {
        // Inform WaveManager
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDied();
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
