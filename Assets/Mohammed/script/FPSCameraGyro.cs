using UnityEngine;
using System.Collections;


public class GunRaycast : MonoBehaviour
{
    public Camera playerCamera;
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.5f;
    public float laserDuration = 0.05f;
    public int scorePerEnemy = 1;
    public AudioSource shootsound;

    private LineRenderer laserLine;
    private float fireTimer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
    }

    
    public void FireGun()
    {
        if (fireTimer < fireRate) return; 
        fireTimer = 0f;

        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        laserLine.SetPosition(0, laserOrigin.position);

        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point);

            
            if (hit.transform.CompareTag("Enemy"))
            {
                Destroy(hit.transform.gameObject);
                shootsound.Play();

                
                SimpleScoreWin scoreSystem = FindObjectOfType<SimpleScoreWin>();
                if (scoreSystem != null)
                {
                    scoreSystem.AddScore(scorePerEnemy);
                }
            }
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
