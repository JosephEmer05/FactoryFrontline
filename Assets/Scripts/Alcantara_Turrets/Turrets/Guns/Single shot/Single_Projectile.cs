using UnityEngine;

public class Single_Projectile : MonoBehaviour
{
    public float speed = 5f;
    public bool homing = true; // if false, move straight forward
    public Alltowerscript owner; // turret providing damage

    private Transform target;

    void Update()
    {
        if (homing)
        {
            if (target == null)
            {
                target = FindNewTarget();
                if (target == null)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    public void SetTarget(Transform newTarget) { target = newTarget; }

    Transform FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null; float minDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance) { minDistance = distance; closest = enemy.transform; }
        }
        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && owner != null)
            enemy.TakeDamage(owner.CurrentDamage);
        Destroy(gameObject);
    }
}
