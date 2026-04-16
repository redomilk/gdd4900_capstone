using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee")]
    public GameObject slashPrefab;
    public float slashDistance = 0.8f;
    public float meleeCooldown = 0.4f;

    [Header("Ranged")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 6f;
    public float rangedCooldown = 0.25f;

    [Header("General")]
    public int currentWeapon = 0;

    float meleeCooldownTimer;
    float actualMeleeCooldown;
    public float MeleeTimerNormalized => actualMeleeCooldown > 0f
        ? Mathf.Clamp01(meleeCooldownTimer / actualMeleeCooldown)
        : 0f;

    float rangedCooldownTimer;
    float actualRangedCooldown;
    public float RangedTimerNormalized => actualRangedCooldown > 0f
        ? Mathf.Clamp01(rangedCooldownTimer / actualRangedCooldown)
        : 0f;

    Camera cam;
    CoreEffects coreEffects;
    CoreInventory inv;

    void Awake()
    {
        cam = Camera.main;
        coreEffects = GetComponent<CoreEffects>();
        inv = GetComponent<CoreInventory>();

        actualMeleeCooldown = meleeCooldown;
        actualRangedCooldown = rangedCooldown;
    }

    void Update()
    {
        meleeCooldownTimer -= Time.deltaTime;
        rangedCooldownTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && rangedCooldownTimer <= 0f)
        {
            AttackRanged();

            float reduction = (inv.rangedCore != null) ? inv.rangedCore.attackCooldownReduction : 0f;
            actualRangedCooldown = Mathf.Max(0.05f, rangedCooldown - reduction);
            rangedCooldownTimer = actualRangedCooldown;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && meleeCooldownTimer <= 0f)
        {
            AttackMelee();

            actualMeleeCooldown = meleeCooldown; // no reduction right now
            meleeCooldownTimer = actualMeleeCooldown;
        }
    }

    void AttackMelee()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(dir * slashDistance);

        GameObject slash = Instantiate(slashPrefab, spawnPos, Quaternion.identity);
        SlashAttack slashAttack = slash.GetComponent<SlashAttack>();
        slashAttack.Initialize(dir);
        slashAttack.SetPlayerPosition(transform.position);
        slashAttack.SetCoreEffects(coreEffects);
    }

    void AttackRanged()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - (Vector3)firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * bulletSpeed;

        PlayerBullet pb = bullet.GetComponent<PlayerBullet>();
        pb.SetCoreEffects(coreEffects);
    }
}