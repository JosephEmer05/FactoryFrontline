using UnityEngine;

public class TestTower : MonoBehaviour
{
    private Alltowerscript turret;

    [Header("Tower Health")]
    public float health = 100f;

    void Awake()
    {
        turret = GetComponent<Alltowerscript>();

        if (turret != null)
        {
            health = turret.CurrentHP;
        }
    }
    public void TakeDamage(float damage)
    {
        if (turret != null)
        {
            damage *= turret.DamageMultiplier;
        }

        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
