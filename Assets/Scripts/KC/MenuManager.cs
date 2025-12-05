using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject towerPartsCanvas;
    [SerializeField] private GameObject campaignCanvas;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button[] backToMenuButtons;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button towerPartsButton;
    [SerializeField] private Button campaignButton;

    private void Start()
    {
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (tutorialCanvas != null) tutorialCanvas.SetActive(false);
        if (towerPartsCanvas != null) towerPartsCanvas.SetActive(false);
        if (campaignCanvas != null) campaignCanvas.SetActive(false);

        if (settingsButton != null) settingsButton.onClick.AddListener(ToggleSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (tutorialButton != null) tutorialButton.onClick.AddListener(OpenTutorial);
        if (towerPartsButton != null) towerPartsButton.onClick.AddListener(OpenTowerParts);
        if (campaignButton != null) campaignButton.onClick.AddListener(OpenCampaign);

        if (backToMenuButtons != null)
        {
            foreach (Button btn in backToMenuButtons)
            {
                if (btn != null)
                    btn.onClick.AddListener(BackToMenu);
            }
        }
    }

    public void ToggleSettings()
    {
        HideAllCanvases();
        if (settingsCanvas != null)
            settingsCanvas.SetActive(true);
    }

    public void BackToMenu()
    {
        HideAllCanvases();
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
    }

    public void OpenTutorial()
    {
        HideAllCanvases();
        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(true);
    }

    public void OpenTowerParts()
    {
        HideAllCanvases();
        if (towerPartsCanvas != null)
            towerPartsCanvas.SetActive(true);
    }

    public void OpenCampaign()
    {
        HideAllCanvases();

        if (campaignCanvas != null)
            campaignCanvas.SetActive(true);

        CampaignManager campaignMgr = campaignCanvas != null ? campaignCanvas.GetComponent<CampaignManager>() : null;
        if (campaignMgr != null)
            campaignMgr.ResetToZoneSelect();
    }

    private void HideAllCanvases()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (tutorialCanvas != null) tutorialCanvas.SetActive(false);
        if (towerPartsCanvas != null) towerPartsCanvas.SetActive(false);
        if (campaignCanvas != null) campaignCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game has been closed");
        Application.Quit();
    }
}
