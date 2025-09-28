using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : MonoBehaviour
{
    [Header("Sword Settings")]
    public Transform sword; // Your 2D sword sprite
    public float attackAngle = 45f; // How much to rotate for attack
    public float attackSpeed = 8f; // How fast the attack happens
    public float retractSpeed = 6f; // How fast it returns
    
    [Header("Attack Detection")]
    public float attackRange = 2f; // How far the sword can hit
    public LayerMask enemyLayer = -1; // Which layers count as enemies
    public int damageAmount = 1; // How much damage per hit
    
    // Store original rotation
    private float originalXRotation;
    private bool isAttacking = false;
    
    void Start()
    {
        // Remember the sword's starting X rotation
        originalXRotation = sword.localEulerAngles.x;
    }
    
    void Update()
    {
        // Click to attack
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(AttackAndRetract());
        }
    }
    
    IEnumerator AttackAndRetract()
    {
        isAttacking = true;
        
        // ATTACK - rotate to attack position
        float timer = 0f;
        float attackTime = 1f / attackSpeed;
        float startRotation = sword.localEulerAngles.x;
        float targetRotation = originalXRotation + attackAngle;
        
        while (timer < attackTime)
        {
            timer += Time.deltaTime;
            float progress = timer / attackTime;
            
            // Smoothly rotate to attack position
            float currentX = Mathf.LerpAngle(startRotation, targetRotation, progress);
            sword.localEulerAngles = new Vector3(currentX, sword.localEulerAngles.y, sword.localEulerAngles.z);
            
            yield return null;
        }
        
        // CHECK FOR ENEMY HIT at peak of attack
        CheckForEnemyHit();
        
        // Small pause at attack position
        yield return new WaitForSeconds(0.05f);
        
        // RETRACT - return to original position
        timer = 0f;
        float retractTime = 1f / retractSpeed;
        startRotation = sword.localEulerAngles.x;
        
        while (timer < retractTime)
        {
            timer += Time.deltaTime;
            float progress = timer / retractTime;
            
            // Smoothly rotate back to original
            float currentX = Mathf.LerpAngle(startRotation, originalXRotation, progress);
            sword.localEulerAngles = new Vector3(currentX, sword.localEulerAngles.y, sword.localEulerAngles.z);
            
            yield return null;
        }
        
        // Make sure we're exactly back to original
        sword.localEulerAngles = new Vector3(originalXRotation, sword.localEulerAngles.y, sword.localEulerAngles.z);
        
        isAttacking = false;
    }
    
    void CheckForEnemyHit()
    {
        // Find all enemies within attack range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        
        // Damage each enemy hit
        foreach (Collider enemyCollider in hitEnemies)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
                Debug.Log($"Hit enemy! Enemy health: {enemyHealth.GetCurrentHealth()}");
            }
        }
    }
    
    // Show attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}