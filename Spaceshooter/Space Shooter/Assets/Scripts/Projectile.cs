using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    
    [Tooltip("The Rigidbody use to move the projectile through the scene")]
    [SerializeField]
    private Rigidbody projectileRigidbody;
    
    [Header("Movement")]
    
    [Tooltip("How quickly the projectile moves through the world. Units per second.")]
    [SerializeField]
    private float movementSpeed = 40f;

    [Tooltip("How long the projectile live for.")]
    [SerializeField]
    private float lifetime = 5f;

    [Header("Damage")]
    [Tooltip("The amount of health removed when the projectile hits a valid target.")]
    [Min(1)]
    [SerializeField]
    private int damageAmount = 1;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //projectileRigidbody.linearVelocity = transform.forward * movementSpeed;
        
        Destroy(gameObject, lifetime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CombatTarget") == false)
        {
            return;
        }
        
        
        bool foundHealth = other.TryGetComponent(out Health targetHealth);//combined definitions in one line
        if (foundHealth)
        {
            targetHealth.TakeDamage(damageAmount);
        }
        
        
        //Debug.Log("Projectile collided with " + other.gameObject.name);
        Destroy(gameObject);
    }


    public void Initialize(Vector3 direction)
    {
        Vector3 normalizedDirection = direction.normalized;
        
        transform.forward = normalizedDirection;
        
        projectileRigidbody.linearVelocity = normalizedDirection * movementSpeed;
        
    }

}
