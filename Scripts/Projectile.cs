using UnityEngine;


public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f; // Projectile speed
    public float lifetime = 5f; // Destroy after 5 seconds
    public float maxDistance = 20f; // Destroy if this far from player
    public float damage = 15f; // Damage dealt to player
    
    private Transform player;
    private Vector3 direction;
    private bool hasRigidbody;
    
    void Start()
    {
        Debug.Log("Projectile created!");
        
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (player != null)
        {
            Debug.Log($"Player found: {player.name}");
            // Calculate direction to player at spawn
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            // If no player found, move forward
            direction = transform.forward;
            Debug.LogWarning("No player found for projectile targeting!");
        }
        
        // Check if we have a rigidbody (used by RangedEnemy)
        Rigidbody rb = GetComponent<Rigidbody>();
        hasRigidbody = (rb != null);
        
        if (hasRigidbody)
        {
            Debug.Log("Projectile using Rigidbody movement");
            // If we have rigidbody, set its velocity instead of using transform
            rb.linearVelocity = direction * speed;
        }
        else
        {
            Debug.Log("Projectile using Transform movement");
        }
        
        // Destroy projectile after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Always face the player if player exists
        Vector3 lookDirection = player.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Destroy if too far
        if (distanceToPlayer > maxDistance)
        {
            Debug.Log($"Projectile destroyed - too far ({distanceToPlayer:F1}m)");
            Destroy(gameObject);
            return;
        }
        
        // Damage player if very close (backup collision detection)
        if (distanceToPlayer < 0.8f)
        {
            Debug.Log("Projectile hit player - close distance damage!");
            DamagePlayer();
            return;
        }
        
        // Only move with transform if we don't have rigidbody
        if (!hasRigidbody)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER HIT: {other.name} with tag '{other.tag}'");
        
        // Check if hit player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Projectile hit player via TRIGGER!");
            DamagePlayer();
            return;
        }
        
        // Destroy on walls/obstacles
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Debug.Log("Projectile hit obstacle via TRIGGER!");
            Destroy(gameObject);
        }
    }
    
    void DamagePlayer()
    {
        // Find and damage the player
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Projectile dealt {damage} damage to player!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found!");
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
    
    
}
