using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Joystick joystick;          // Reference to the joystick
    public float speedMove = 5f;       // Movement speed
    public Transform cameraTransform;  // Reference to the main camera

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get Rigidbody component
    }

    private void FixedUpdate()
    {
        // Get joystick input
        Vector3 input = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        // Skip if no input
        if (input.magnitude < 0.01f)
            return;

        // Make movement relative to camera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Movement direction relative to camera
        Vector3 move = camForward * input.z + camRight * input.x;

        // Apply movement with physics
        rb.MovePosition(rb.position + move * speedMove * Time.fixedDeltaTime);
    }
}
