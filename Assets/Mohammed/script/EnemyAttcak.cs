using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float speed = 5f;
    private Transform player;
    private HealthSystem playerHealth;

    void Start()
    {
        // Find the object with tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthSystem>(); // Get HealthSystem from player
        }
        else
        {
            Debug.LogWarning("No object with tag 'Player' found in the scene!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Find direction to player
        Vector3 direction = (player.position - transform.position).normalized;

        // Rotate enemy to face player
        transform.rotation = Quaternion.LookRotation(direction);

        // Move toward player
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerHealth != null)
        {
            playerHealth.TakeDamage(10);  // Call the HealthSystem on player
            Destroy(gameObject);
        }
    }
}
