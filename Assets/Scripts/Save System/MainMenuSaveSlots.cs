using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuSaveSlots : MonoBehaviour
{
    [Header("Panels")]
    public GameObject saveSlotPanel;

    [Header("Buttons")]
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    [Header("Button Labels")]
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;
    public TMP_Text slot3Text;

    [Header("Scene")]
    public string firstRunSceneName = "Game";

    void Start()
    {
        saveSlotPanel.SetActive(false);

        RefreshSlotLabels();

        slot1Button.onClick.AddListener(() => SelectSlot(1));
        slot2Button.onClick.AddListener(() => SelectSlot(2));
        slot3Button.onClick.AddListener(() => SelectSlot(3));
    }

    public void OnPlayPressed()
    {
        saveSlotPanel.SetActive(true);
    }

    void SelectSlot(int slot)
    {
        GameManager.instance.LoadGame(slot);
        SceneManager.LoadScene(firstRunSceneName);
    }

    void RefreshSlotLabels()
    {
        slot1Text.text = SaveManager.SaveExists(1) ? "Slot 1 - Continue" : "Slot 1 - New Game";
        slot2Text.text = SaveManager.SaveExists(2) ? "Slot 2 - Continue" : "Slot 2 - New Game";
        slot3Text.text = SaveManager.SaveExists(3) ? "Slot 3 - Continue" : "Slot 3 - New Game";
    }
}