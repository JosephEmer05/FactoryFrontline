using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Scene Settings")]
    public string menuSceneName = "MainMenu";
    public string nextLevelSceneName = "";

    [Header("Enemy Counter UI")]
    public Slider enemySlider;
    public TMP_Text enemyCounterText;

    [Header("Wave Timer UI")]
    public TMP_Text waveTimerText;

    [Header("Screens")]
    public GameObject winScreen;
    public GameObject gameOverScreen;

    [Header("Buttons")]
    public Button winNextLevelButton;
    public Button winRestartButton;
    public Button winMenuButton;

    public Button loseRetryButton;
    public Button loseMenuButton;

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

    public void SetEnemySlider(int total, int remaining)
    {
        enemySlider.minValue = 0;
        enemySlider.maxValue = total;
        enemySlider.value = remaining;

        if (enemyCounterText != null)
            enemyCounterText.text = $"{remaining}/{total}";
    }

    public void UpdateEnemySlider(int remaining)
    {
        enemySlider.value = remaining;

        if (enemyCounterText != null)
            enemyCounterText.text = $"{remaining}/{(int)enemySlider.maxValue}";

        if (enemySlider.value <= 0)
            ShowWinScreen();
    }

    public void UpdateWaveTimer(float time)
    {
        if (waveTimerText != null)
            waveTimerText.text = Mathf.CeilToInt(time).ToString();
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0f;
        winScreen.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
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
        {
            Debug.LogError("UIManager Error: Menu scene name is empty.");
            return;
        }

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

        // Fallback: load next scene by build index
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(next);
        }
        else
        {
            Debug.Log("No more levels are listed in Build Settings.");
        }
    }
}
