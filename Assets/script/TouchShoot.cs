using UnityEngine;

public class TouchShoot : MonoBehaviour
{
    [Header("Settings")]
    public float rayDistance = 100f;          // How far the raycast goes
    public LayerMask hitLayers;              // Which layers can be hit
    public GameObject hitEffectPrefab;       // Optional particle effect on hit

    void Update()
    {
        // Only works if there is at least one touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Shoot();
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // For testing in editor with mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
#endif
    }

    void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, hitLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Spawn hit effect if assigned
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Destroy the hit object
            Destroy(hit.collider.gameObject);
        }
    }
}
