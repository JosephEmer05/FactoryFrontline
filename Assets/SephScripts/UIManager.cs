using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public string menuSceneName = "";
    public string nextLevelSceneName = "";

    public Slider enemySlider;
    public TMP_Text enemyCounterText;

    public TMP_Text baseHealthText;

    public TMP_Text waveTimerText;

    public GameObject winScreen;
    public GameObject gameOverScreen;

    public Button winNextLevelButton;
    public Button winRestartButton;
    public Button winMenuButton;

    public Button loseRetryButton;
    public Button loseMenuButton;

    public float winCountdownSeconds = 3f;
    private Coroutine winCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (winRestartButton != null)
            winRestartButton.onClick.AddListener(RestartLevel);

        if (winMenuButton != null)
            winMenuButton.onClick.AddListener(() => LoadMenu());

        if (winNextLevelButton != null)
            winNextLevelButton.onClick.AddListener(() => LoadNextLevel());

        if (loseRetryButton != null)
            loseRetryButton.onClick.AddListener(RestartLevel);

        if (loseMenuButton != null)
            loseMenuButton.onClick.AddListener(() => LoadMenu());
    }

    public void SetBaseHealth(float maxHP)
    {
        if (baseHealthText != null)
            baseHealthText.text = maxHP.ToString();
    }

    public void UpdateBaseHealth(float hp)
    {
        if (baseHealthText != null)
            baseHealthText.text = hp.ToString();
    }

    public void SetEnemySlider(int total, int remaining)
    {
        enemySlider.minValue = 0;
        enemySlider.maxValue = total;
        enemySlider.value = remaining;

        if (enemyCounterText != null)
            enemyCounterText.text = remaining + "/" + total;
    }

    public void UpdateEnemySlider(int remaining)
    {
        enemySlider.value = remaining;

        if (enemyCounterText != null)
            enemyCounterText.text = remaining + "/" + (int)enemySlider.maxValue;

        if (enemySlider.value <= 0 && winCoroutine == null)
            winCoroutine = StartCoroutine(WinCountdownAndShow());
    }

    IEnumerator WinCountdownAndShow()
    {
        float t = winCountdownSeconds;

        while (t > 0f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            t -= 0.1f;
        }

        if (enemySlider.value <= 0)
        {
            Time.timeScale = 0f;
            winScreen.SetActive(true);
        }

        winCoroutine = null;
    }

    public void UpdateWaveTimer(float time)
    {
        if (waveTimerText != null)
            waveTimerText.text = Mathf.CeilToInt(time).ToString();
    }

    public void ShowGameOverScreen()
    {
        if (winCoroutine != null)
        {
            StopCoroutine(winCoroutine);
            winCoroutine = null;
        }

        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(menuSceneName))
            return;

        SceneManager.LoadScene(menuSceneName);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
            return;
        }

        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
    }
}
