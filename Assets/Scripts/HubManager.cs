using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HubManager : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "Main"; // match your scene name
    public TextMeshProUGUI scrapText;

    void Start()
    {
        RefreshScrap();
    }

    void RefreshScrap()
    {
        if (scrapText != null && GameManager.instance != null)
            scrapText.text = "" + GameManager.instance.scrapCount;
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}