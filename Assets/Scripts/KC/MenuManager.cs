/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource; //AudioSource for button click SFX
    [SerializeField] private AudioClip buttonClickClip; //Button click sound clip
    [SerializeField][Range(0f, 1f)] private float buttonSfxVolume = 0.5f; //Volume for button click SFX

    private void Awake()
    {
        if (buttonAudioSource == null)
        {
            buttonAudioSource = gameObject.AddComponent<AudioSource>();
            buttonAudioSource.playOnAwake = false;
            buttonAudioSource.loop = false;
            buttonAudioSource.spatialBlend = 0f; //2D sound for UI

            //Assign SFX mixer group
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                buttonAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
            else
            {
                Debug.LogWarning("MenuManager could not find AudioManager or AudioMixer. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
    }

    private void Start()
    {
        settingsCanvas.SetActive(false);

        settingsButton.onClick.AddListener(ToggleSettings);
        backToMenuButton.onClick.AddListener(ToggleMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void ToggleSettings()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void ToggleMainMenu()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }
        Debug.Log("Game has been closed");
        Application.Quit();
    }
}
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject comingSoonCanvas;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button endlessButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button towerPartsButton;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField][Range(0f, 1f)] private float buttonSfxVolume = 0.5f;

    private Coroutine comingSoonCoroutine;

    private void Awake()
    {
        if (buttonAudioSource == null)
        {
            buttonAudioSource = gameObject.AddComponent<AudioSource>();
            buttonAudioSource.playOnAwake = false;
            buttonAudioSource.loop = false;
            buttonAudioSource.spatialBlend = 0f;

            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                buttonAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
        }
    }

    private void Start()
    {
        settingsCanvas.SetActive(false);
        comingSoonCanvas.SetActive(false);

        settingsButton.onClick.AddListener(ToggleSettings);
        backToMenuButton.onClick.AddListener(ToggleMainMenu);
        quitButton.onClick.AddListener(QuitGame);

        endlessButton.onClick.AddListener(ShowComingSoon);
        shopButton.onClick.AddListener(ShowComingSoon);
        towerPartsButton.onClick.AddListener(ShowComingSoon);
    }

    private void PlayClickSound()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
    }

    public void ToggleSettings()
    {
        PlayClickSound();
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void ToggleMainMenu()
    {
        PlayClickSound();
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void ShowComingSoon()
    {
        PlayClickSound();
        comingSoonCanvas.SetActive(true);

        //Restart timer if already running
        if (comingSoonCoroutine != null)
        {
            StopCoroutine(comingSoonCoroutine);
        }
        comingSoonCoroutine = StartCoroutine(HideComingSoonAfterDelay());
    }

    private IEnumerator HideComingSoonAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        comingSoonCanvas.SetActive(false);
        comingSoonCoroutine = null;
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Game has been closed");
        Application.Quit();
    }
}
