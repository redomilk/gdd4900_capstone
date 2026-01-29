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
    public float entrySinkImpulse = 3f;    // pro tip: extra downward impulse on entry (0 = off)

    [Header("Swim Movement (Water Only)")]
    public float swimAcceleration = 35f;
    public float maxHorizontalSpeed = 4.5f;
    public float maxVerticalSpeed = 4.5f;
    public float swimStopDamping = 10f; // higher = stops faster when no input

    [Header("Optional Upward Drift (0 = off)")]
    public float upwardDriftForce = 0f; // try 0.5 - 2 for gentle float up

    Rigidbody2D rb;
    bool inWater;

    bool sinking;
    float sinkTimer;

    void Awake() => rb = GetComponent<Rigidbody2D>();

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

        // Entry sink phase: no swim control yet, let the player sink a bit
        if (sinking)
        {
            sinkTimer -= Time.fixedDeltaTime;

            if (sinkTimer <= 0f)
            {
                sinking = false;

                // Switch to normal swim physics
                rb.gravityScale = waterGravityScale;
                rb.linearDamping = waterDamping;
            }

            return;
        }

        // Normal swim
        Vector2 input = GetMoveInput();
        ApplySwimMovement(input);

        if (upwardDriftForce > 0f)
            rb.AddForce(Vector2.up * upwardDriftForce, ForceMode2D.Force);
    }

    void EnterAir()
    {
        inWater = false;
        sinking = false;

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
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }
}