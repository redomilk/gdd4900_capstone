using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ExtractSummaryUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text headerText;

    [SerializeField] private TMP_Text scrapCollectedValue;
    [SerializeField] private TMP_Text scrapLostValue;
    [SerializeField] private TMP_Text totalScrapValue;
    [SerializeField] private TMP_Text timeValue;

    [SerializeField] private string hubSceneName = "HUB";

    void Start()
    {
        var gm = GameManager.instance;
        if (gm == null) return;

        headerText.text = gm.lastRunExtracted ? "EXTRACT SUCCESS" : "DIVE FAILED";

        scrapCollectedValue.text = gm.lastRunScrapCollected.ToString();
        scrapLostValue.text = gm.lastRunScrapLost.ToString();
        totalScrapValue.text = gm.scrapCount.ToString();
        timeValue.text = FormatTime(gm.lastRunTime);

        headerText.color = gm.lastRunExtracted ? Color.green : Color.red;
    }

    public void GoToHub()
    {
        SceneManager.LoadScene(hubSceneName);
    }

    string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}