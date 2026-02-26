using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HubShop : MonoBehaviour
{
    [Header("Scrap Display")]
    public TextMeshProUGUI scrapCountText;
    [Header("HP Upgrade")]
    public Image hpProgressBar;
    public TextMeshProUGUI hpLevelText;
    public TextMeshProUGUI hpCostText;
    [Header("Oxygen Upgrade")]
    public Image oxygenProgressBar;
    public TextMeshProUGUI oxygenLevelText;
    public TextMeshProUGUI oxygenCostText;
    [Header("Speed Upgrade")]
    public Image spdProgressBar;
    public TextMeshProUGUI spdLevelText;
    public TextMeshProUGUI spdCostText;
    const int maxLevel = 10;
    const int costPerLevel = 20;
    void Start()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        if (GameManager.instance == null) return;
        scrapCountText.text = "" + GameManager.instance.scrapCount;
        UpdateUpgradeUI(hpProgressBar, hpLevelText, hpCostText, GameManager.instance.healthLevel, "HP");
        UpdateUpgradeUI(oxygenProgressBar, oxygenLevelText, oxygenCostText, GameManager.instance.oxygenLevel, "Oxygen");
        UpdateUpgradeUI(spdProgressBar, spdLevelText, spdCostText, GameManager.instance.speedLevel, "Speed");
    }
    void UpdateUpgradeUI(Image bar, TextMeshProUGUI levelText, TextMeshProUGUI costText, int currentLevel, string upgradeName)
    {
        if (bar != null) bar.fillAmount = (float)currentLevel / maxLevel;
        if (levelText != null) levelText.text = upgradeName + " Level " + currentLevel + " / " + maxLevel;
        if (costText != null) costText.text = currentLevel >= maxLevel ? "MAXED" : "Cost: " + costPerLevel + " scrap";
    }
    public void BuyHPUpgrade()
    {
        if (TryPurchase(ref GameManager.instance.healthLevel))
        {
            Debug.Log("HP upgraded to level " + GameManager.instance.healthLevel);
            RefreshUI();
        }
    }
    public void BuyOxygenUpgrade()
    {
        if (TryPurchase(ref GameManager.instance.oxygenLevel))
        {
            Debug.Log("Oxygen upgraded to level " + GameManager.instance.oxygenLevel);
            RefreshUI();
        }
    }
    public void BuySpdUpgrade()
    {
        if (TryPurchase(ref GameManager.instance.speedLevel))
        {
            Debug.Log("Speed upgraded to level " + GameManager.instance.speedLevel);
            RefreshUI();
        }
    }
    bool TryPurchase(ref int level)
    {
        if (level >= maxLevel) { Debug.Log("Already maxed!"); return false; }
        if (GameManager.instance.scrapCount < costPerLevel) { Debug.Log("Not enough scrap!"); return false; }
        GameManager.instance.scrapCount -= costPerLevel;
        level++;
        return true;
    }
}