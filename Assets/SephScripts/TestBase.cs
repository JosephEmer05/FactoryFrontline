using UnityEngine;

public class TestBase : MonoBehaviour
{
    public float basehealth = 200f;

    public void TakeDamage(float damage)
    {
        basehealth -= damage;
        Debug.Log("Base took damage! Current HP: " + basehealth);
        if (basehealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        UIManager.Instance.ShowGameOverScreen();
        Debug.Log("Base Destroyed! Game Over!");
    }
}
