using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 2; // Enemy dies after 2 hits
    
    private int currentHealth;
    
    void Start()
    {
        // Set health to maximum at start
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        // Reduce health
        currentHealth -= damage;
        
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Check if enemy should die
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    void Die()
    {
        Debug.Log("Enemy died!");
        
        // Add death effects here if needed
        // For example: play death animation, spawn particles, etc.
        
        // Destroy the enemy
        Destroy(gameObject);
    }
}