using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{

    public enum MonsterState
    {
        Patrolling,
        Chasing
    }

    [SerializeField] float lookRadius = 10f;
    Transform playerTransform;
    NavMeshAgent agent;
    public MonsterState currentState = MonsterState.Patrolling;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform currentWaypoint;
    [SerializeField] float distanceToWaypoint;
    [SerializeField] int waypointIndex;

    private void Start()
    {
        playerTransform = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        GetRandomWaypoint();
    }

    private void Update()
    {
        if (currentState == MonsterState.Chasing)
        {
            //float distance = Vector3.Distance(target.position, transform.position);
            //if (distance <= lookRadius)
            //{
            //    agent.SetDestination(target.position);

            //    if (distance <= agent.stoppingDistance)
            //    {
            //        FaceTarget();
            //    }
            //}
        }
        else
        {
            if (currentWaypoint)
            {
                
                
                
                if (agent.remainingDistance < 2f)
                {
                    UpdateWaypoint();
                }
            }
        }

    }

    void UpdateWaypoint()
    {

        if (waypoints.Length <= waypointIndex +1)
        {
            waypointIndex = 0;
            currentWaypoint = waypoints[0];

        }
        else
        {
            waypointIndex += 1;
            currentWaypoint = waypoints[waypointIndex];

        }
        FaceTarget(currentWaypoint);
        agent.SetDestination(currentWaypoint.position);

    }

    void GetRandomWaypoint()
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        waypointIndex = randomIndex;
        currentWaypoint = waypoints[waypointIndex];
        agent.SetDestination(currentWaypoint.position);
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
