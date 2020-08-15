using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmasherAnimation : MonoBehaviour
{

    Animator animator;
    NavMeshAgent agent;
    SmasherController smasherController;
    Vector3 previousPosition;
    Rigidbody rb;
    [SerializeField] Transform backPackEffectPoint;
    [SerializeField] GameObject backPackEffectObject;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        smasherController = GetComponent<SmasherController>();
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
        {
            Vector3 currentMove = transform.position - previousPosition;
            float currentSpeed = currentMove.magnitude / Time.deltaTime;
            float speedPercent = currentSpeed / agent.speed;
            if (smasherController.currentState == SmasherController.MonsterState.Patrolling)
            {
                animator.SetFloat("Speed", speedPercent);
            }
            else
            {
                animator.SetFloat("Speed", speedPercent);
            }
            
        }
        if (rb.velocity.y < 0 && !smasherController.GetbGrounded())
        {
            animator.SetBool("Falling", true);
        }
        previousPosition = transform.position;
    }

    public void Attack()
    {
        animator.SetBool("UpperBodyAction", true);
        animator.SetTrigger("Attack");
    }

    public void StopAttack()
    {
        animator.SetBool("UpperBodyAction", false);
    }
    public void Jumped()
    {
        
        animator.SetBool("Grounded", false);
        GameObject backPackEffect = null;
        backPackEffect = Instantiate(backPackEffectObject, backPackEffectPoint.position, backPackEffectPoint.rotation,backPackEffectPoint);
        Destroy(backPackEffect, 1f);
    }

    public void Landed()
    {
        animator.SetBool("Falling", false);
        animator.SetBool("Grounded", true);
    }
}