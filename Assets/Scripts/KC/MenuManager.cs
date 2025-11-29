using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    //[SerializeField] private GameObject playCanvas;
    [SerializeField] private GameObject towerPartsCanvas;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button[] backToMenuButtons;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button towerPartsButton;
    [SerializeField] private Button playButton;

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
        }
    }

    private void Start()
    {
        settingsCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
      //  playCanvas.SetActive(false);
        towerPartsCanvas.SetActive(false);

        settingsButton.onClick.AddListener(ToggleSettings);
        quitButton.onClick.AddListener(QuitGame);

        foreach (Button btn in backToMenuButtons)
        {
            btn.onClick.AddListener(BackToMenu);
        }

        tutorialButton.onClick.AddListener(OpenTutorial);
       // playButton.onClick.AddListener(OpenPlay);
        towerPartsButton.onClick.AddListener(OpenTowerParts);
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
        HideAllCanvases();
        settingsCanvas.SetActive(true);
    }

    public void BackToMenu()
    {
        PlayClickSound();
        HideAllCanvases();
        mainMenuCanvas.SetActive(true);
    }

    public void OpenTutorial()
    {
        PlayClickSound();
        HideAllCanvases();
        tutorialCanvas.SetActive(true);
    }

    /*public void OpenPlay()
    {
        PlayClickSound();
        HideAllCanvases();
        playCanvas.SetActive(true);
    }*/

    public void OpenTowerParts()
    {
        PlayClickSound();
        HideAllCanvases();
        towerPartsCanvas.SetActive(true);
    }

    private void HideAllCanvases()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
       // playCanvas.SetActive(false);
        towerPartsCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Game has been closed");
        Application.Quit();
    }
}
