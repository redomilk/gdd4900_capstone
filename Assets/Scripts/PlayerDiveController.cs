using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDiveController : MonoBehaviour
{
    public float gravityScaleAir = 2.5f;

    [Header("Water Feel")]
    public float waterGravityScale = 0.0f;   // set to 0 for true free swim
    public float waterDamping = 8f;

    [Header("Water Entry Sink")]
    public float entrySinkTime = 0.4f;     // how long to sink before swim control
    public float entrySinkGravity = 0.8f;  // gravity while sinking
    public float entrySinkDamping = 4f;    // damping while sinking
    public float entrySinkImpulse = 3f;    // extra downward impulse on entry (0 = off)

    [Header("Swim Movement (Water Only)")]
    public float swimAcceleration = 35f;
    public float maxHorizontalSpeed = 4.5f;
    public float maxVerticalSpeed = 4.5f;
    public float swimStopDamping = 10f; // higher = stops faster when no input

    [Header("Optional Upward Drift (0 = off)")]
    public float upwardDriftForce = 0f; // try 0.5 - 2 for gentle float up

    [Header("Swim Boost")]
    public float boostCooldown = 0.6f;   // time between boosts
    public float boostDuration = 0.25f;  // how long boost "wins" over swim
    public float boostMaxSpeed = 12f;    // speed during boost (separate from normal max speeds)
    public float boostDamping = 0f;      // damping while boosting (0 = preserve speed)

    [Header("Boost Oxygen Cost (uses PlayerStats)")]
    public float boostOxygenCost = 20f;

    [Header("Boost Particles")]
    public ParticleSystem boostParticles;

    Rigidbody2D rb;
    PlayerStats stats;

    bool inWater;

    // Entry sink
    bool sinking;
    float sinkTimer;

    // Boost state
    bool boosting;
    float boostTimeLeft;
    float boostTimer;
    float savedDamping;
    Vector2 lastInputDir = Vector2.down; // default direction if no input yet
    bool boostQueued; //tracks if boost was pressed

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>(); // expects PlayerStats on same GameObject
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

    void FixedUpdate()
    {
        if (!inWater) return;
        
        //rotate to follow only in water
        RotateTowardMouse();

        boostTimer -= Time.fixedDeltaTime;

        // Entry sink phase: no swim control yet, let the player sink a bit
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

        // Remember last non-zero direction for boosting
        if (input.sqrMagnitude > 0f)
            lastInputDir = input.normalized;

        // Boost input can happen anytime in water (except sink phase)
        HandleBoost(input);

        if (boosting)
        {
            boostTimeLeft -= Time.fixedDeltaTime;

            if (boostTimeLeft <= 0f)
            {
                boosting = false;

                // restore normal water damping (or saved value if you prefer)
                rb.linearDamping = waterDamping;
            }

            // While boosting: DO NOT apply normal swim movement/clamps
            ClampVelocity(boostMaxSpeed, boostMaxSpeed);
        }
        else
        {
            // Normal swim
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

        // Use keyboard direction if held, otherwise face direction
        Vector2 boostDir = input.sqrMagnitude > 0f ? input.normalized : (Vector2)transform.up;

        boosting = true;
        boostTimeLeft = boostDuration;
        savedDamping = rb.linearDamping;
        rb.linearDamping = boostDamping;
        rb.linearVelocity = boostDir * boostMaxSpeed;
        boostTimer = boostCooldown;

        if (boostParticles != null)
        {
            // Point particles opposite to boost direction
            Debug.Log("Playing particles");
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

        // optional: remove any sideways drift carryover from water
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void EnterWater()
    {
        inWater = true;

        // optional: prevent upward pop on entry
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 0f));

        // Start sink phase
        sinking = true;
        sinkTimer = entrySinkTime;

        boosting = false;

        rb.gravityScale = entrySinkGravity;
        rb.linearDamping = entrySinkDamping;

        // Pro tip: a tiny downward impulse makes entry feel juicy
        if (entrySinkImpulse > 0f)
            rb.AddForce(Vector2.down * entrySinkImpulse, ForceMode2D.Impulse);
    }

    // -------------------
    // Movement (New Input System)
    // -------------------
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
            // no input -> slow down smoothly (water resistance)
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * swimStopDamping);
        }

        // Clamp speed per-axis so vertical feels as free as horizontal
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