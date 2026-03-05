using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDiveController : MonoBehaviour
{
    public float gravityScaleAir = 2.5f;

    [Header("Water Feel")]
    public float waterGravityScale = 0.0f;
    public float waterDamping = 8f;

    [Header("Water Entry Sink")]
    public float entrySinkTime = 0.4f;
    public float entrySinkGravity = 0.8f;
    public float entrySinkDamping = 4f;
    public float entrySinkImpulse = 3f;

    [Header("Swim Movement (Water Only)")]
    public float swimAcceleration = 35f;
    public float maxHorizontalSpeed = 4.5f;
    public float maxVerticalSpeed = 4.5f;
    public float swimStopDamping = 10f;

    [Header("Optional Upward Drift (0 = off)")]
    public float upwardDriftForce = 0f;

    [Header("Swim Boost")]
    public float boostCooldown = 0.6f;
    public float boostDuration = 0.25f;
    public float boostMaxSpeed = 12f;
    public float boostDamping = 0f;

    [Header("Boost Oxygen Cost (uses PlayerStats)")]
    public float boostOxygenCost = 20f;

    [Header("Boost Particles")]
    public ParticleSystem boostParticles;

    Rigidbody2D rb;
    PlayerStats stats;

    bool inWater;

    bool sinking;
    float sinkTimer;

    bool boosting;
    float boostTimeLeft;
    float boostTimer;
    float savedDamping;
    Vector2 lastInputDir = Vector2.down;
    bool boostQueued;

    // Knockback
    float knockbackTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    void OnEnable()
    {
        GameEvents.OnPlayerHitWater += EnterWater;
        GameEvents.OnPlayerLeftWater += EnterAir;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerHitWater -= EnterWater;
        GameEvents.OnPlayerLeftWater -= EnterAir;
    }

    void Start()
    {
        EnterAir();
        GameEvents.OnPlayerStartFalling?.Invoke();
    }

    public void ApplyKnockback(float duration)
    {
        knockbackTimer = duration;
        boosting = false;
        rb.linearDamping = waterDamping;  // ensure damping is active during knockback
    }

    void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            //bleed off knockback velocity so it doesn't carry forever
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * waterDamping);

            if (knockbackTimer <= 0f)
                rb.linearVelocity = Vector2.zero;  // hard reset when knockback ends

            return;
        }

        if (!inWater) return;

        RotateTowardMouse();

        boostTimer -= Time.fixedDeltaTime;

        if (sinking)
        {
            sinkTimer -= Time.fixedDeltaTime;

            if (sinkTimer <= 0f)
            {
                sinking = false;
                rb.gravityScale = waterGravityScale;
                rb.linearDamping = waterDamping;
            }

            return;
        }

        Vector2 input = GetMoveInput();

        if (input.sqrMagnitude > 0f)
            lastInputDir = input.normalized;

        HandleBoost(input);

        if (boosting)
        {
            boostTimeLeft -= Time.fixedDeltaTime;

            if (boostTimeLeft <= 0f)
            {
                boosting = false;
                rb.linearDamping = waterDamping;
            }

            ClampVelocity(boostMaxSpeed, boostMaxSpeed);
        }
        else
        {
            ApplySwimMovement(input);
        }

        if (upwardDriftForce > 0f)
            rb.AddForce(Vector2.up * upwardDriftForce, ForceMode2D.Force);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            boostQueued = true;
    }

    void HandleBoost(Vector2 input)
    {
        if (!boostQueued) return;
        boostQueued = false;

        if (boostTimer > 0f) return;

        if (boostOxygenCost > 0f)
        {
            if (stats == null) return;
            if (!stats.TrySpendOxygen(boostOxygenCost)) return;
        }

        Vector2 boostDir = input.sqrMagnitude > 0f ? input.normalized : (Vector2)transform.up;

        boosting = true;
        boostTimeLeft = boostDuration;
        savedDamping = rb.linearDamping;
        rb.linearDamping = boostDamping;
        rb.linearVelocity = boostDir * boostMaxSpeed;
        boostTimer = boostCooldown;

        if (boostParticles != null)
        {
            boostParticles.transform.rotation = Quaternion.LookRotation(Vector3.forward, -boostDir);
            boostParticles.Play();
        }
    }

    void EnterAir()
    {
        inWater = false;
        sinking = false;
        boosting = false;

        rb.gravityScale = gravityScaleAir;
        rb.linearDamping = 0f;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void EnterWater()
    {
        inWater = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 0f));

        sinking = true;
        sinkTimer = entrySinkTime;

        boosting = false;

        rb.gravityScale = entrySinkGravity;
        rb.linearDamping = entrySinkDamping;

        if (entrySinkImpulse > 0f)
            rb.AddForce(Vector2.down * entrySinkImpulse, ForceMode2D.Impulse);
    }

    Vector2 GetMoveInput()
    {
        if (Keyboard.current == null) return Vector2.zero;

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;
        if (Keyboard.current.wKey.isPressed) y += 1f;

        Vector2 v = new Vector2(x, y);
        return v.sqrMagnitude > 1f ? v.normalized : v;
    }

    void ApplySwimMovement(Vector2 input)
    {
        if (input.sqrMagnitude > 0f)
        {
            rb.AddForce(input * swimAcceleration, ForceMode2D.Force);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * swimStopDamping);
        }

        ClampVelocity(maxHorizontalSpeed, maxVerticalSpeed);
    }

    void ClampVelocity(float maxX, float maxY)
    {
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxX, maxX);
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -maxY, maxY);
        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }

    void RotateTowardMouse()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mouseWorld - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}