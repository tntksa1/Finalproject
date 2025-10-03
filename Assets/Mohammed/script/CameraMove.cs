using UnityEngine;

public class CamGyro : MonoBehaviour
{
    private GameObject camParent;
    public Transform player; // Reference to player

    void Awake()
    {
        // Create a parent object to handle yaw separately
        camParent = new GameObject("CamParent");
        camParent.transform.position = transform.position;

        // Make the camera child of the new parent
        transform.SetParent(camParent.transform);

        // Enable the gyroscope
        if (SystemInfo.supportsGyroscope)
            Input.gyro.enabled = true;
        else
            Debug.LogWarning("Gyroscope not supported on this device!");
    }

    void Update()
    {
        if (!SystemInfo.supportsGyroscope) return;

        // Make CamParent follow player
        if (player != null)
            camParent.transform.position = player.position;

        // Rotate the parent on Y axis (yaw)
        camParent.transform.Rotate(0, -Input.gyro.rotationRateUnbiased.y, 0);

        // Rotate the camera on X axis (pitch)
        transform.Rotate(-Input.gyro.rotationRateUnbiased.x, 0, 0);
    }

} 