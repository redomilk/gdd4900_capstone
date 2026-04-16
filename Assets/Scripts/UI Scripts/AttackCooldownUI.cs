using UnityEngine;
using UnityEngine.UI;

public class AttackCooldownUI : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack playerAttack;
    public Image meleeOverlayImage;
    public Image rangedOverlayImage;

    void Update()
    {
        meleeOverlayImage.fillAmount = playerAttack.MeleeTimerNormalized;
        rangedOverlayImage.fillAmount = playerAttack.RangedTimerNormalized;
    }
}