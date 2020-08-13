using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public enum PlayerCombatState
    {
        Attacking,
        Serenading,
        Shielding,
        None
    }

    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerController playerController;
    [SerializeField] LayerMask attackableLayers;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage = 40;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackAnimationDelay = .15f;
    [SerializeField] float shieldingDelay = 1.2f;
    [SerializeField] float shieldingTime = .75f;
    [SerializeField] GameObject[] serenadeEffects;
    public PlayerCombatState currentState;
    float nextAttackTime = 0f;
    bool bSwingAttack = false;
    bool bSerenading = false;
    bool bShielding = false;
    float lastShieldTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerCombatState.None;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!bSerenading && !bShielding)
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
            if (!bSwingAttack && playerController.GetbGrounded() && !bShielding)
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!bSerenading && !bShielding && !bSwingAttack && Time.time - lastShieldTime >= shieldingDelay)
            {
                StartShielding();
                lastShieldTime = Time.time;
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
        currentState = PlayerCombatState.Attacking;
        playerAnimation.PlayerAttacked();
        StartCoroutine("SwingAttackDelayRoutine");
    }

    public void SwingAttack()
    {
        
        //Detect objects in range of attack
        Collider[] attackedColliders = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayers);
        foreach (Collider collider in attackedColliders)
        {
            HealthManager healthManager = collider.gameObject.GetComponent<HealthManager>();
            if (healthManager)
            {
                if (healthManager is SmasherHealthManager)
                {
                    SmasherHealthManager smasherHealthManager = (SmasherHealthManager)healthManager;

                    if (Vector3.Angle(smasherHealthManager.transform.forward,transform.forward) > 90)
                    {
                        //hit smasher in the front so do less damage
                        healthManager.TakeDamage(10);
                    }
                    else
                    {
                        smasherHealthManager.TakeDamage(100);
                    }
                }
                else if (healthManager is MonsterHealthManager)
                {
                    MonsterHealthManager monsterHealthManager = (MonsterHealthManager)healthManager;
                    monsterHealthManager.TakeDamage(100);
                }
                
            }

            
        }
        currentState = PlayerCombatState.None;
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
        currentState = PlayerCombatState.Serenading;
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
            currentState = PlayerCombatState.None;
            bSerenading = false;
            playerAnimation.PlayerStoppedSerenading();
            foreach (GameObject obj in serenadeEffects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void StartShielding()
    {
        bShielding = true;
        currentState = PlayerCombatState.Shielding;
        playerController.movementEnabled = false;
        playerAnimation.PlayerStartedShielding();
        StartCoroutine("ShieldRoutine");
    }

    public void StopShielding()
    {
        playerAnimation.PlayerStoppedShielding();
        playerController.movementEnabled = true;
        currentState = PlayerCombatState.None;
        bShielding = false;
    }
    IEnumerator ShieldRoutine()
    {
        yield return new WaitForSeconds(shieldingTime);
        StopShielding();
    }

    
}
