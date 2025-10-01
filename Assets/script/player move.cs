using UnityEngine;

public class playermove : MonoBehaviour
{
    Rigidbody rb;
    float dirX;
    [SerializeField] float moveSpeed = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get accelerometer input
        dirX = Input.acceleration.x * moveSpeed;

        // Clamp position so Player stays inside bounds
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -7.5f, 7.5f),
            transform.position.y
        );
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.linearVelocity = new Vector2(dirX, 0f);
    }
}