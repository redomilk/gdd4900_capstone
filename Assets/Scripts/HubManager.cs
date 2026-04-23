using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HubManager : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "SQ scene";
    public TextMeshProUGUI scrapText;

    void Start()
    {
        // Bank any remaining run scrap when returning to hub normally
       // if (GameManager.instance != null)
           // GameManager.instance.BankRunScrap();

        RefreshScrap();
    }

    void RefreshScrap()
    {
        if (scrapText != null && GameManager.instance != null)
            scrapText.text = "" + GameManager.instance.scrapCount;  // shows stash
    }

    public void GoToMainScene()
    {
        if (GameManager.instance != null)
            GameManager.instance.StartRun();

        SceneManager.LoadScene(mainSceneName);
    }
}