using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float preferredDistance = 8f; // Distance to maintain from player
    public float minDistance = 6f; // If closer than this, back away
    
    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Your projectile prefab
    public Transform shootPoint; // Where projectiles spawn from
    public float shootCooldown = 2f; // Time between shots
    public float projectileSpeed = 10f;
    public float projectileDamage = 15f; // Damage for spawned projectiles
    
    [Header("References")]
    public Transform spriteTransform;
    
    private Transform player;
    private float lastShotTime = 0f;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (spriteTransform == null)
            spriteTransform = transform;
        
        if (shootPoint == null)
            shootPoint = transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        FacePlayer();
        MaintainDistance();
        TryShoot();
    }
    
    void FacePlayer()
    {
        Vector3 directionToPlayer = player.position - spriteTransform.position;
        directionToPlayer.y = 0f;
        
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            spriteTransform.rotation = Quaternion.Slerp(spriteTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void MaintainDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer < minDistance)
        {
            // Too close, back away
            Vector3 awayDirection = (transform.position - player.position).normalized;
            awayDirection.y = 0f;
            transform.position += awayDirection * moveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer > preferredDistance + 2f)
        {
            // Too far, move closer (but not too close)
            Vector3 towardDirection = (player.position - transform.position).normalized;
            towardDirection.y = 0f;
            transform.position += towardDirection * moveSpeed * 0.5f * Time.deltaTime;
        }
    }
    
    void TryShoot()
    {
        if (Time.time - lastShotTime >= shootCooldown)
        {
            ShootAtPlayer();
            lastShotTime = Time.time;
        }
    }
    
    void ShootAtPlayer()
    {
        if (projectilePrefab == null) 
        {
            Debug.LogWarning("No projectile prefab assigned!");
            return;
        }
        
        Debug.Log("Ranged enemy shoots!");
        
        // Create projectile
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        
        // Set projectile damage if it has the Projectile component
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.damage = projectileDamage;
        }
        
        Debug.Log($"Projectile spawned with {projectileDamage} damage");
    }

    public void Die()
    {
        Debug.Log("Ranged enemy died!");
        Destroy(gameObject);
    }
}
