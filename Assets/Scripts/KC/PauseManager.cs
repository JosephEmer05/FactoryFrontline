using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseButton;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField][Range(0f, 1f)] private float buttonSfxVolume = 0.5f;

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
            else
            {
                Debug.LogWarning("PauseManager could not find AudioManager or AudioMixer.");
            }
        }
    }

    private void Start()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //If paused, resume. If not paused, pause.
            if (pauseCanvas.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PlayClickSound()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
    }

    public void PauseGame()
    {
        PlayClickSound();
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        PlayClickSound();
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
