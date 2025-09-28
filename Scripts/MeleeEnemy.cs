using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackDistance = 2f; // How close to attack player
    
    [Header("Attack Settings")]
    public float attackDamage = 20f; // Damage per attack
    public float attackCooldown = 1.5f; // Time between attacks
    
    [Header("References")]
    public Transform spriteTransform;
    
    private Transform player;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (player == null)
        {
            Debug.LogWarning("No player found! Make sure player has 'Player' tag.");
        }
        
        if (spriteTransform == null)
            spriteTransform = transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        FacePlayer();
        MoveAndAttack();
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
    
    void MoveAndAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= attackDistance)
        {
            // Close enough to attack
            if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            // Move toward player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    
    System.Collections.IEnumerator AttackPlayer()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        Debug.Log("Melee enemy attacks player!");
        
        // Find and damage the player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Melee enemy dealt {attackDamage} damage to player!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on player!");
        }
        
        // Brief pause for attack animation
        yield return new WaitForSeconds(0.3f);
        
        isAttacking = false;
    }
    
    public void Die()
    {
        Debug.Log("Melee enemy died!");
        Destroy(gameObject);
    }
}
