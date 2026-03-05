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
    public int currentWeapon = 0;

    float cooldown;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0f) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AttackRanged();
            cooldown = attackCooldown;
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            AttackMelee();
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
        SlashAttack slashAttack = slash.GetComponent<SlashAttack>();
        slashAttack.Initialize(dir);
        slashAttack.SetPlayerPosition(transform.position);
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