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
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        smasherController = GetComponent<SmasherController>();
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentMove = transform.position - previousPosition;
        float currentSpeed = currentMove.magnitude / Time.deltaTime;
        float speedPercent = currentSpeed / agent.speed;
        if (smasherController.currentState == SmasherController.MonsterState.Patrolling)
        {
            animator.SetFloat("Speed", speedPercent / 2f);
        }
        else
        {
            animator.SetFloat("Speed", speedPercent);
        }
        previousPosition = transform.position;
    }
}