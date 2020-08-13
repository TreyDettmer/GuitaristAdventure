using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmasherHealthManager : HealthManager
{

    [SerializeField] GameObject backpack;
    [SerializeField] Transform ExplosionPoint;
    [SerializeField] GameObject explosionEffect;

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
            if (damage == 100)
            {
                Die(true);
            }
            else
            {
                Die();
            }

        }

    }

    void Die(bool explode = false)
    {
        base.Die();
        SmasherController smasherController = gameObject.GetComponentInParent<SmasherController>();
        smasherController.TurnOnRagdoll();
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("DeadMonster"));
        healthBarObject.SetActive(false);
        GameObject explosionEffectInstance = null;
        if (explode)
        {
            explosionEffectInstance = Instantiate(explosionEffect, ExplosionPoint.position, ExplosionPoint.rotation);
            if (backpack)
            {
                Destroy(backpack);
            }
        }
        if (explosionEffectInstance != null)
        {
            Destroy(explosionEffectInstance, 3f);
        }
        Destroy(gameObject, 3f);
        
                
        
    }
}
