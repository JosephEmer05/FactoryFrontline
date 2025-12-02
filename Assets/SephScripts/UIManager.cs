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

    private bool uiReady = false;

    private bool pendingBaseHealth = false;
    private float pendingBaseHealthValue;

    private bool pendingEnemySliderSet = false;
    private int pendingEnemyTotal;
    private int pendingEnemyRemaining;

    private bool pendingEnemySliderUpdate = false;
    private int pendingEnemyRemainingUpdate;

    private bool pendingWaveTimer = false;
    private float pendingWaveTimerValue;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    IEnumerator Start()
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

        yield return null;
        uiReady = true;

        if (pendingBaseHealth)
        {
            baseHealthText.text = pendingBaseHealthValue.ToString();
            pendingBaseHealth = false;
        }

        if (pendingEnemySliderSet)
        {
            enemySlider.minValue = 0;
            enemySlider.maxValue = pendingEnemyTotal;
            enemySlider.value = pendingEnemyRemaining;
            if (enemyCounterText != null)
                enemyCounterText.text = pendingEnemyRemaining + "/" + pendingEnemyTotal;
            pendingEnemySliderSet = false;
        }

        if (pendingEnemySliderUpdate)
        {
            enemySlider.value = pendingEnemyRemainingUpdate;
            if (enemyCounterText != null)
                enemyCounterText.text = pendingEnemyRemainingUpdate + "/" + (int)enemySlider.maxValue;
            if (enemySlider.value <= 0 && winCoroutine == null)
                winCoroutine = StartCoroutine(WinCountdownAndShow());
            pendingEnemySliderUpdate = false;
        }

        if (pendingWaveTimer)
        {
            waveTimerText.text = Mathf.CeilToInt(pendingWaveTimerValue).ToString();
            pendingWaveTimer = false;
        }
    }

    public void SetBaseHealth(float maxHP)
    {
        if (!uiReady || baseHealthText == null)
        {
            pendingBaseHealth = true;
            pendingBaseHealthValue = maxHP;
            return;
        }
        baseHealthText.text = maxHP.ToString();
    }

    public void UpdateBaseHealth(float hp)
    {
        if (!uiReady || baseHealthText == null)
        {
            pendingBaseHealth = true;
            pendingBaseHealthValue = hp;
            return;
        }
        baseHealthText.text = hp.ToString();
    }

    public void SetEnemySlider(int total, int remaining)
    {
        if (!uiReady || enemySlider == null)
        {
            pendingEnemySliderSet = true;
            pendingEnemyTotal = total;
            pendingEnemyRemaining = remaining;
            return;
        }

        enemySlider.minValue = 0;
        enemySlider.maxValue = total;
        enemySlider.value = remaining;

        if (enemyCounterText != null)
            enemyCounterText.text = remaining + "/" + total;
    }

    public void UpdateEnemySlider(int remaining)
    {
        if (!uiReady || enemySlider == null)
        {
            pendingEnemySliderUpdate = true;
            pendingEnemyRemainingUpdate = remaining;
            return;
        }

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

        if (enemySlider != null && enemySlider.value <= 0)
        {
            Time.timeScale = 0f;
            if (winScreen != null) winScreen.SetActive(true);
        }

        winCoroutine = null;
    }

    public void UpdateWaveTimer(float time)
    {
        if (!uiReady || waveTimerText == null)
        {
            pendingWaveTimer = true;
            pendingWaveTimerValue = time;
            return;
        }
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
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
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
