using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject slashPrefab;

    public float attackCooldown = 0.25f;
    public float slashDistance = 0.8f;

    float cooldown;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (Mouse.current.leftButton.wasPressedThisFrame && cooldown <= 0f)
        {
            Attack();
            cooldown = attackCooldown;
        }
    }

    void Attack()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - transform.position).normalized;

        Vector3 spawnPos = transform.position + (Vector3)(dir * slashDistance);

        GameObject slash = Instantiate(slashPrefab, spawnPos, Quaternion.identity);

        slash.GetComponent<SlashAttack>().Initialize(dir);
    }
}