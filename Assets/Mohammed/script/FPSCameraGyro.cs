using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class GunRaycast : MonoBehaviour
{
    public Camera playerCamera;
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.5f;
    public float laserDuration = 0.05f;

    private LineRenderer laserLine;
    private float fireTimer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // update timer every frame
        fireTimer += Time.deltaTime;
    }

    // 🔫 Call this method from a UI Button
    public void FireGun()
    {
        if (fireTimer < fireRate) return; // cooldown
        fireTimer = 0f;

        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        laserLine.SetPosition(0, laserOrigin.position);

        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point);
            Destroy(hit.transform.gameObject); // destroy object on hit
        }
        else
        {
            laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * gunRange));
        }

        StartCoroutine(ShootLaser());
    }

    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
}
