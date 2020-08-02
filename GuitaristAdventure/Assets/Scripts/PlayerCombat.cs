using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerController playerController;
    [SerializeField] LayerMask attackableLayers;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage = 40;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackAnimationDelay = .15f;
    [SerializeField] GameObject[] serenadeEffects;
    float nextAttackTime = 0f;
    bool bSwingAttack = false;
    bool bSerenading = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!bSerenading)
            {
                if (Time.time >= nextAttackTime)
                {

                    StartSwingAttack();
                    nextAttackTime = Time.time + 1 / attackRate;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!bSwingAttack && playerController.GetbGrounded())
            {
                if (bSerenading)
                {
                    StopSerenading();

                }
                else
                {
                    StartSerenading();
                }
            }
        }
        if (!playerController.GetbGrounded() && bSerenading)
        {
            bSerenading = false;
        }
        
    }

    void StartSwingAttack()
    {
        bSwingAttack = true;
        playerAnimation.PlayerAttacked();
        StartCoroutine("SwingAttackDelayRoutine");
    }

    public void SwingAttack()
    {
        
        //Detect objects in range of attack
        Collider[] attackedColliders = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayers);
        foreach (Collider collider in attackedColliders)
        {

            collider.GetComponent<HealthManager>().TakeDamage(attackDamage);
        }
        bSwingAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    IEnumerator SwingAttackDelayRoutine()
    {
        yield return new WaitForSeconds(attackAnimationDelay);
        SwingAttack();
    }

    public void StartSerenading()
    {
        bSerenading = true;
        playerAnimation.PlayerStartedSerenading();
        foreach (GameObject obj in serenadeEffects)
        {
            obj.SetActive(true);
        }
    }

    public void StopSerenading()
    {
        if (bSerenading)
        {
            bSerenading = false;
            playerAnimation.PlayerStoppedSerenading();
            foreach (GameObject obj in serenadeEffects)
            {
                obj.SetActive(false);
            }
        }
    }

    
}
