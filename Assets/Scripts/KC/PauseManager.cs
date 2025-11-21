using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button resumeButton;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource; //Audio source for button click SFX
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
            resumeButton.onClick.AddListener(TogglePause);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }

        bool shouldPause = !pauseCanvas.activeSelf;
        pauseCanvas.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f; //Resets time when destroyed
    }
}
