using NUnit;
using UnityEngine;
using UnityEngine.UI;

public class CoreHUDIcons : MonoBehaviour
{
    public static CoreHUDIcons instance;

    [Header("HUD Icon Images")]
    public Image mainIcon;
    public Image meleeIcon;
    public Image rangedIcon;
    public Image boosterIcon;

    [Header("Empty Slot Sprite (optional)")]
    public Sprite emptySprite;

    private CoreInventory inv;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            inv = player.GetComponent<CoreInventory>();

        Refresh();
    }

    public void Refresh()
    {
        if (inv == null)
        {
            Debug.LogWarning("CoreHUDIcons: inv is null!");
            return;
        }

        /*Debug.Log($"CoreHUDIcons Refresh — " +
            $"Main: {(inv.mainCore != null ? inv.mainCore.coreName : "null")} icon={inv.mainCore?.icon?.name ?? "null"} | " +
            $"Melee: {(inv.meleeCore != null ? inv.meleeCore.coreName : "null")} icon={inv.meleeCore?.icon?.name ?? "null"} | " +
            $"Ranged: {(inv.rangedCore != null ? inv.rangedCore.coreName : "null")} icon={inv.rangedCore?.icon?.name ?? "null"} | " +
            $"Booster: {(inv.boosterCore != null ? inv.boosterCore.coreName : "null")} icon={inv.boosterCore?.icon?.name ?? "null"}");

            Debug.Log($"CoreHUDIcons Image refs — " +
            $"mainIcon={mainIcon?.name ?? "null"} | " +
            $"meleeIcon={meleeIcon?.name ?? "null"} | " +
            $"rangedIcon={rangedIcon?.name ?? "null"} | " +
            $"boosterIcon={boosterIcon?.name ?? "null"}");
        */

        SetIcon(mainIcon, inv.mainCore);
        SetIcon(meleeIcon, inv.meleeCore);
        SetIcon(rangedIcon, inv.rangedCore);
        SetIcon(boosterIcon, inv.boosterCore);
    }

    void SetIcon(Image img, CoreData core)
    {
        if (img == null) return;
        if (core != null && core.icon != null)
        {
            img.sprite = core.icon;
            img.color = Color.white;
        }
        else
        {
            img.sprite = emptySprite;
            img.color = emptySprite != null ? Color.white : new Color(1, 1, 1, 0.2f);
        }
    }
}