using UnityEngine;

public enum AugmentSlot { Core, Arms, Legs }

[CreateAssetMenu(fileName = "AugmentData", menuName = "Scriptable Objects/AugmentData")]
public class AugmentData : ScriptableObject
{
    [Header("Identity")]
    public string augmentName;
    public AugmentSlot slot;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Stat Modifiers")]
    public float healthBonus = 0f;
    public float speedBonus = 0f;
    public float damageBonus = 0f;
    public float oxygenBonus = 0f;
}
