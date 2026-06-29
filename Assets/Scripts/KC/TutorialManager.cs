using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Pages (Parents)")]
    [SerializeField] private GameObject[] pages = new GameObject[7];

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentPage = 0;

    private void Start()
    {
        if (pages.Length == 0)
        {
            Debug.LogError("TutorialManager: No pages assigned!");
            return;
        }

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == null)
                Debug.LogError("TutorialManager: A page slot is empty!");
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);
    }

    private void OnEnable()
    {
        ShowPage(currentPage);
    }

    public void ResetTutorial()
    {
        currentPage = 0;
        ShowPage(currentPage);
    }

    private void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Length)
            return;

        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(false);

        pages[index].SetActive(true);
        currentPage = index;

        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPage > 0);

        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
    }

    public void NextPage()
    {
        int next = currentPage + 1;
        if (next < pages.Length)
            ShowPage(next);
    }

    public void PreviousPage()
    {
        int prev = currentPage - 1;
        if (prev >= 0)
            ShowPage(prev);
    }
}
