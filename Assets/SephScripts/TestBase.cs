using UnityEngine;

public class TestBase : MonoBehaviour
{
    public float basehealth = 10f;

    void Start()
    {
        UIManager.Instance.SetBaseHealth(basehealth);
    }

    public void TakeDamage(float damage)
    {
        basehealth -= damage;
        UIManager.Instance.UpdateBaseHealth(basehealth);

        if (basehealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        UIManager.Instance.ShowGameOverScreen();
    }
}
