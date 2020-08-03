using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimation : MonoBehaviour
{

    Animator animator;
    NavMeshAgent agent;
    MonsterController monsterController;
    Vector3 previousPosition;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        monsterController = GetComponent<MonsterController>();
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentMove = transform.position - previousPosition;
        float currentSpeed = currentMove.magnitude / Time.deltaTime;
        float speedPercent = currentSpeed / agent.speed;
        if (monsterController.currentState == MonsterController.MonsterState.Patrolling)
        {
            animator.SetFloat("Speed", speedPercent /2f);
        }
        else
        {
            animator.SetFloat("Speed", speedPercent);
        }
        previousPosition = transform.position;
    }
}
