using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LavaPool : MonoBehaviour
{
    [Header("Lava Damage Settings")]
    public float damagePerSecond = 10f; // How much damage per second
    public float damageInterval = 0.5f; // How often to deal damage (every 0.5 seconds)
    public bool instantDamage = false; // Deal damage immediately on touch
    public float instantDamageAmount = 5f; // Damage for instant hit
    
    [Header("Visual/Audio (Optional)")]
    public ParticleSystem damageEffect; // Particle effect when player takes damage
    public AudioSource damageSound; // Sound when player takes damage
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private bool playerInLava = false;
    private PlayerHealth playerHealth;
    private Coroutine damageCoroutine;
    
    void Start()
    {
        // Make sure this has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"Lava pool '{gameObject.name}' should have 'Is Trigger' checked!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered the lava
        if (other.CompareTag("Player"))
        {
            if (showDebugMessages)
                Debug.Log("Player entered lava pool!");
            
            playerInLava = true;
            playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth == null)
            {
                Debug.LogWarning("Player doesn't have PlayerHealth component!");
                return;
            }
            
            // Deal instant damage if enabled
            if (instantDamage)
            {
                playerHealth.TakeDamage(instantDamageAmount);
                if (showDebugMessages)
                    Debug.Log($"Lava dealt {instantDamageAmount} instant damage!");
                
            }
            
            // Start continuous damage
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ContinuousDamage());
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if player left the lava
        if (other.CompareTag("Player"))
        {
            if (showDebugMessages)
                Debug.Log("Player left lava pool!");
            
            playerInLava = false;
            playerHealth = null;
            
            // Stop continuous damage
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }
    
    IEnumerator ContinuousDamage()
    {
        while (playerInLava && playerHealth != null)
        {
            // Wait for the damage interval
            yield return new WaitForSeconds(damageInterval);
            
            // Deal damage if player is still in lava
            if (playerInLava && playerHealth != null)
            {
                float damageAmount = damagePerSecond * damageInterval;
                playerHealth.TakeDamage(damageAmount);
                
                if (showDebugMessages)
                    Debug.Log($"Lava dealt {damageAmount} continuous damage!");
                
                // Check if player died
                if (playerHealth.GetCurrentHealth() <= 0)
                {
                    if (showDebugMessages)
                        Debug.Log("Player died in lava!");
                    break;
                }
            }
        }
        
        damageCoroutine = null;
    }
    
    
    // Public method to check if player is currently in lava
    public bool IsPlayerInLava()
    {
        return playerInLava;
    }
    
    // Public method to manually trigger damage (for testing)
    public void TestDamage()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond);
            Debug.Log("Manual lava damage triggered!");
        }
    }
    
    // Show lava area in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        
        // Draw the collider bounds
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
        
        // Draw damage info
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * 2f);
    }
}