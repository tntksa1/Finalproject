using UnityEngine;

/// <summary>
/// Gyro-backed first-person camera controller with touch/mouse fallback.
/// Attach to the Camera. If you have a player body, rotate it on the Y (yaw)
/// and let this script handle pitch on the camera (recommended).
/// </summary>
[RequireComponent(typeof(Camera))]
public class GyroFPCamera : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("Use device gyroscope when available (mobile). Otherwise use touch/mouse fallback.")]
    public bool allowGyro = true;

    [Header("Sensitivity & Smoothing")]
    public float gyroSensitivity = 1.0f;        // multiplier applied to gyro reading
    public float mouseSensitivity = 3.0f;       // mouse/touch sensitivity for fallback
    [Range(0f, 1f)] public float smoothing = 0.85f; // higher = smoother

    [Header("Pitch (X) Limits")]
    public bool clampPitch = true;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("References")]
    [Tooltip("Optional: a Transform representing the player's body (rotates yaw). If null the camera's parent will be used if present.")]
    public Transform playerBody;

    // internal state
    Quaternion gyroInitialRotation = Quaternion.identity;
    Quaternion baseRotation = Quaternion.Euler(90, 0, 0); // adjustment for gyro coordinate space
    bool gyroSupported;
    Quaternion smoothRotation;
    Vector2 currentMouseDelta;

    // mouse/touch state
    Vector2 touchLookVelocity;
    Vector2 accumulatedEuler; // x = pitch, y = yaw

    void Start()
    {
        // Determine player body
        if (playerBody == null && transform.parent != null)
            playerBody = transform.parent;

        // Cursor lock for desktop
#if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif

        // Gyro setup
        gyroSupported = SystemInfo.supportsGyroscope && allowGyro;
        if (gyroSupported)
        {
            Input.gyro.enabled = true;
            // Save initial orientation state so we can zero/align camera
            gyroInitialRotation = Input.gyro.attitude;
            // compute smoothRotation initial
            smoothRotation = transform.localRotation;
        }
        else
        {
            // initialize accumulated Euler from current transforms
            Vector3 e = transform.localEulerAngles;
            accumulatedEuler.x = NormalizeAngle(e.x);
            accumulatedEuler.y = playerBody ? NormalizeAngle(playerBody.localEulerAngles.y) : NormalizeAngle(transform.localEulerAngles.y);
            smoothRotation = transform.localRotation;
        }
    }

    void Update()
    {
        if (gyroSupported)
            UpdateGyro();
        else
            UpdateFallback();

        // Optional: keep pitch clamped by adjusting camera.localEulerAngles after smoothing
        if (clampPitch && !gyroSupported)
        {
            float clampedPitch = Mathf.Clamp(accumulatedEuler.x, minPitch, maxPitch);
            accumulatedEuler.x = clampedPitch;
        }
    }

    void UpdateGyro()
    {
        // Read gyro attitude and convert to Unity space
        Quaternion deviceRotation = GyroToUnityQuaternion(Input.gyro.attitude);

        // Calculate offset from initial orientation to keep current device orientation aligned
        Quaternion relative = deviceRotation * Quaternion.Inverse(GyroToUnityQuaternion(gyroInitialRotation));

        // Optionally apply a base rotation if device axes differ (90 deg correction often needed)
        Quaternion targetLocal = baseRotation * relative;

        // Apply sensitivity by slerping toward target with a sensitivity factor
        Quaternion target = Quaternion.Slerp(transform.localRotation, targetLocal, Mathf.Clamp01((1f - smoothing) * gyroSensitivity * Time.deltaTime * 60f));

        // Optionally clamp pitch (harder with quaternions — convert to euler to clamp)
        if (clampPitch)
        {
            Vector3 e = target.eulerAngles;
            float pitch = NormalizeAngle(e.x);
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            e.x = pitch;
            target = Quaternion.Euler(e);
        }

        transform.localRotation = target;
    }

    void UpdateFallback()
    {
        // Mouse on Editor/Standalone, touch on mobile
        Vector2 lookDelta = Vector2.zero;

        // Mouse
#if UNITY_EDITOR || UNITY_STANDALONE
        lookDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
#else
        // Touch: single-finger drag
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                lookDelta = t.deltaPosition * (mouseSensitivity * 0.02f); // scale touch delta to feel like mouse
            }
        }
#endif

        // Accumulate yaw on player body, pitch on camera
        accumulatedEuler.y += lookDelta.x;
        accumulatedEuler.x -= lookDelta.y; // invert for natural look

        if (clampPitch)
            accumulatedEuler.x = Mathf.Clamp(accumulatedEuler.x, minPitch, maxPitch);

        // Smooth the rotations
        // Smooth yaw on player body
        if (playerBody != null)
        {
            float targetYaw = accumulatedEuler.y;
            float currentYaw = NormalizeAngle(playerBody.localEulerAngles.y);
            float smoothedYaw = Mathf.LerpAngle(currentYaw, targetYaw, 1f - smoothing);
            playerBody.localRotation = Quaternion.Euler(0f, smoothedYaw, 0f);
        }
        else
        {
            // If no player body, rotate camera's yaw too (not recommended for proper FPS movement)
            float targetYaw = accumulatedEuler.y;
            float currentYaw = NormalizeAngle(transform.localEulerAngles.y);
            float smoothedYaw = Mathf.LerpAngle(currentYaw, targetYaw, 1f - smoothing);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, smoothedYaw, 0f);
        }

        // Smooth pitch on camera
        float currentPitch = NormalizeAngle(transform.localEulerAngles.x);
        float smoothedPitch = Mathf.LerpAngle(currentPitch, accumulatedEuler.x, 1f - smoothing);
        transform.localRotation = Quaternion.Euler(smoothedPitch, transform.localEulerAngles.y, 0f);
    }

    /// <summary>
    /// Convert gyro quaternion to Unity coordinate system and correct handedness.
    /// </summary>
    static Quaternion GyroToUnityQuaternion(Quaternion q)
    {
        // Input.gyro.attitude uses a different coordinate system: (x, y, z, w)
        // Typical conversion:
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    static float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a > 180f) a -= 360f;
        if (a < -180f) a += 360f;
        return a;
    }

    // Public helper: recenter gyro baseline (useful if player rotates device and wants to reset view)
    public void RecenterGyro()
    {
        if (!gyroSupported) return;
        gyroInitialRotation = Input.gyro.attitude;
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    // Optional: unlock cursor with Escape for testing
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
#endif
}
