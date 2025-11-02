using UnityEngine;

public class Single_Projectile : MonoBehaviour
{
    public float speed = 5f;
    private Transform target;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy missile if target died
            return;
        }

        // Move toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Face the direction of movement
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; // Assigns the target from the turret
    }

    Transform FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    // 3D collision event
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // Destroy enemy
            Destroy(gameObject);        // Destroy projectile
        }
    }
}
