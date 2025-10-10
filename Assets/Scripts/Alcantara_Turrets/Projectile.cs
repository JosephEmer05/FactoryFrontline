using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float detectionRadius = 0.5f;
    private Vector3 direction;

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
