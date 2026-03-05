using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HubManager : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "Main";
    public TextMeshProUGUI scrapText;

    void Start()
    {
        // Bank any remaining run scrap when returning to hub normally
        if (GameManager.instance != null)
            GameManager.instance.BankRunScrap();

        RefreshScrap();
    }

    void RefreshScrap()
    {
        if (scrapText != null && GameManager.instance != null)
            scrapText.text = "" + GameManager.instance.scrapCount;  // shows stash
    }

    public void GoToMainScene()
    {
        // Reset run scrap for new run
        if (GameManager.instance != null)
            GameManager.instance.runScrapCount = 0;

        SceneManager.LoadScene(mainSceneName);
    }
}