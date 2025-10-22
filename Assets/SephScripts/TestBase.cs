using UnityEngine;

public class TestBase : MonoBehaviour
{
    public float health = 200f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Base took damage! Current HP: " + health);
        if (health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Base Destroyed! Game Over!");
    }
}
