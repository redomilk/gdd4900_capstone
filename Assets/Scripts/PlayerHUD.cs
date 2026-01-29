using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHUD : MonoBehaviour
{
    public Image healthFill;
    public Image oxygenFill;

    void OnEnable()
    {
        GameEvents.OnHealthChanged += HandleHealthChanged;
        GameEvents.OnOxygenChanged += HandleOxygenChanged;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    void OnDisable()
    {
        GameEvents.OnHealthChanged -= HandleHealthChanged;
        GameEvents.OnOxygenChanged -= HandleOxygenChanged;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }

    void HandleHealthChanged(float current, float max)
    {
        if (healthFill) healthFill.fillAmount = (max <= 0f) ? 0f : current / max;
    }

    void HandleOxygenChanged(float current, float max)
    {
        if (oxygenFill) oxygenFill.fillAmount = (max <= 0f) ? 0f : current / max;
    }

    void HandlePlayerDied()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
