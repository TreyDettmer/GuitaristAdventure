using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmasherHealthManager : HealthManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        currentHealth -= damage;

        ChangeHealthDisplay();

        //TODO: play animation

        if (currentHealth <= 0)
        {
            //die
            Die();

        }

    }

    protected override void Die()
    {
        base.Die();
        SmasherController smasherController = gameObject.GetComponentInParent<SmasherController>();
        smasherController.TurnOnRagdoll();
        healthBarObject.SetActive(false);
        Destroy(gameObject, 3f);
                
        
    }
}
