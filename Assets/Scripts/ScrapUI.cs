using UnityEngine;
using TMPro;

public class ScrapUI : MonoBehaviour
{
    public TextMeshProUGUI scrapText;
    PlayerStats player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (player != null)
            scrapText.text = $"{player.scrapCount}";
    }
}