﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] LayerMask attackableLayers;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage = 40;
    [SerializeField] float attackRate = 2f;
    float nextAttackTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                
                Attack();
                nextAttackTime = Time.time + 1 / attackRate;
            }
        }
    }

    void Attack()
    {
        //TODO: play attack animation and do attack using animation event

        //Detect objects in range of attack
        Collider[] attackedColliders = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayers);
        foreach (Collider collider in attackedColliders)
        {
            
            collider.GetComponent<HealthManager>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
