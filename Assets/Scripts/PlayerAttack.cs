using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee")]
    public GameObject slashPrefab;
    public float slashDistance = 0.8f;

    [Header("Ranged")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 6f;

    [Header("General")]
    public float attackCooldown = 0.25f;

    // UI hook - you can display this
    public int currentWeapon = 0; // 0 = melee, 1 = ranged

    float cooldown;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Scroll to swap weapon
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) currentWeapon = 0;
        if (scroll < 0f) currentWeapon = 1;

        cooldown -= Time.deltaTime;
        if (Mouse.current.leftButton.wasPressedThisFrame && cooldown <= 0f)
        {
            if (currentWeapon == 0) AttackMelee();
            else AttackRanged();
            cooldown = attackCooldown;
        }
    }

    void AttackMelee()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(dir * slashDistance);
        GameObject slash = Instantiate(slashPrefab, spawnPos, Quaternion.identity);
        slash.GetComponent<SlashAttack>().Initialize(dir);
    }

    void AttackRanged()
    {
        if (bulletPrefab == null || firePoint == null) return;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - (Vector3)firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }
}