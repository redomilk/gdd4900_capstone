using UnityEngine;
using UnityEngine.UI;

public class BoostCooldownUI : MonoBehaviour
{
    [Header("References")]
    public PlayerDiveController diveController;
    public Image overlayImage;

    void Update()
    {
        overlayImage.fillAmount = diveController.BoostTimerNormalized;
    }
}