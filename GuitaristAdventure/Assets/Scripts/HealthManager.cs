using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //TODO: play animation

        if (currentHealth <= 0)
        {
            //die
            Die();
            
        }
    }

    void Die()
    {
        if (tag != "Player")
        {
            Destroy(gameObject);
        }
    }

    
}
