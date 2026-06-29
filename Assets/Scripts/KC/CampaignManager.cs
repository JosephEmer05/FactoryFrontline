using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject zoneSelectPanel;
    [SerializeField] private GameObject zone1LevelsPanel;
    [SerializeField] private GameObject zone2LevelsPanel;

    private void OnEnable()
    {
        ResetToZoneSelect();
    }

    public void OpenZone1()
    {
        if (zoneSelectPanel != null) zoneSelectPanel.SetActive(false);
        if (zone1LevelsPanel != null) zone1LevelsPanel.SetActive(true);
        if (zone2LevelsPanel != null) zone2LevelsPanel.SetActive(false);
    }

    public void OpenZone2()
    {
        if (zoneSelectPanel != null) zoneSelectPanel.SetActive(false);
        if (zone1LevelsPanel != null) zone1LevelsPanel.SetActive(false);
        if (zone2LevelsPanel != null) zone2LevelsPanel.SetActive(true);
    }

    public void BackToZoneSelect()
    {
        ResetToZoneSelect();
    }

    public void ResetToZoneSelect()
    {
        if (zoneSelectPanel != null)
            zoneSelectPanel.SetActive(true);

        if (zone1LevelsPanel != null)
            zone1LevelsPanel.SetActive(false);

        if (zone2LevelsPanel != null)
            zone2LevelsPanel.SetActive(false);
    }
}
