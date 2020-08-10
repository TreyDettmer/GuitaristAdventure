﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class SmasherController : MonoBehaviour
{

    public enum MonsterState
    {
        Patrolling,
        Chasing,
        Idling,
        Dead

    }

    [SerializeField] bool bShouldPatrol = false;
    [SerializeField] float defaultLookDistance = 10f;
    Transform playerTransform;
    NavMeshAgent agent;
    public MonsterState currentState = MonsterState.Patrolling;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform currentWaypoint;
    int waypointIndex;
    bool bIdling = false;
    [SerializeField] int idlePercentChance = 80;
    [SerializeField] float minIdleTime, maxIdleTime;
    [SerializeField] float fireRateMin, fireRateMax;
    float lastFireTime = 0f;
    float fireRate = 1f;

    [SerializeField] LayerMask sightLayers;
    [SerializeField] Transform headTransform;

    float lastPlayerAppearance = 0f;
    [SerializeField] float suspicionTime = 4f;
    [SerializeField] float suspicionLookDistance = 40f;
    [SerializeField] List<Collider> ragdollParts = new List<Collider>();

    private void Awake()
    {
        SetRagdollParts();
    }

    private void Start()
    {
        playerTransform = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        if (bShouldPatrol)
        {
            GetRandomWaypoint();
        }
        
    }

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                c.enabled = false;
                ragdollParts.Add(c);
            }
        }

    }

    public void TurnOnRagdoll()
    {

        currentState = MonsterState.Dead;
        agent.isStopped = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;


        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<Animator>().avatar = null;

        foreach (Collider c in ragdollParts)
        {
            c.enabled = true;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.attachedRigidbody.AddForce(-transform.forward * 7f, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool bCanSeePlayer = false;
        RaycastHit hit;
        switch (currentState)
        {
            case MonsterState.Chasing:

                bCanSeePlayer = false;
                //Check that we can still see the player
                if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, suspicionLookDistance, sightLayers))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {

                        bCanSeePlayer = true;
                        //Continue to chase
                        if (Vector3.Angle(playerTransform.position - transform.position, transform.forward) >= 90)
                        {
                            FaceTarget(playerTransform);
                        }
                        agent.SetDestination(playerTransform.position);
                        //don't point gun at exact player location when player is close. Doesn't look good.
                        if (distance > 2.5f)
                        {

                        }
                    }
                }
                //If we can't see the player, then stop chasing.
                if (bCanSeePlayer == false)
                {
                    lastPlayerAppearance = Time.time;

                    currentState = MonsterState.Idling;
                    //keep facing the same direction for a while in case the player reappears
                    Idle(suspicionTime);
                }
                else
                {
                    //Shoot
                    if (Time.time - lastFireTime > fireRate)
                    {

                        ShootWeapon();
                        fireRate = Random.Range(fireRateMin, fireRateMax);
                        lastFireTime = Time.time;
                    }
                }


                break;
            case MonsterState.Idling:
                if (bIdling == false)
                {
                    Idle();

                }
                bCanSeePlayer = false;
                //If we recently saw the player then look for a further distance
                if (Time.time - lastPlayerAppearance <= suspicionTime)
                {
                    if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, suspicionLookDistance, sightLayers))
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            bCanSeePlayer = true;
                        }
                    }
                }
                else //otherwise look the default look distance
                {

                    if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, defaultLookDistance, sightLayers))
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            bCanSeePlayer = true;
                        }
                    }
                }
                //If we can see the player, then start chasing
                if (bCanSeePlayer)
                {
                    currentState = MonsterState.Chasing;
                    StopCoroutine("IdleRoutine");
                    agent.isStopped = false;
                    bIdling = false;
                }



                break;
            case MonsterState.Patrolling:
                if (bShouldPatrol)
                {
                    if (currentWaypoint)
                    {
                        //Check if we have reached the current waypoint
                        if (agent.remainingDistance < 2f)
                        {
                            //Go to the next waypoint
                            UpdateWaypoint();
                        }
                    }
                    //Check if we can see the player
                    bCanSeePlayer = false;
                    if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, defaultLookDistance, sightLayers))
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            bCanSeePlayer = true;
                        }
                    }
                    //If we can see the player, then start chasing
                    if (bCanSeePlayer)
                    {
                        currentState = MonsterState.Chasing;
                    }
                }
                break;
        }



    }

    void UpdateWaypoint()
    {
        if (Random.Range(1, 100) >= 100 - idlePercentChance)
        {

            currentState = MonsterState.Idling;
        }
        else
        {
            if (waypoints.Length <= waypointIndex + 1)
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

    }

    void GetRandomWaypoint()
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        waypointIndex = randomIndex;
        currentWaypoint = waypoints[waypointIndex];
        FaceTarget(currentWaypoint);
        agent.SetDestination(currentWaypoint.position);
    }

    void Idle(float setTime = -1f)
    {
        bIdling = true;
        if (setTime > -1f)
        {
            StartCoroutine("IdleRoutine", setTime);
        }
        else
        {
            StartCoroutine("IdleRoutine", Random.Range(minIdleTime, maxIdleTime));
        }

        agent.isStopped = true;
    }

    void ShootWeapon()
    {


    }


    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (new Vector3(direction.x, 0, direction.z) != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }



    }

    IEnumerator IdleRoutine(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        GetRandomWaypoint();
        agent.isStopped = false;
        currentState = MonsterState.Patrolling;
        bIdling = false;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, defaultLookDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, suspicionLookDistance);
    }
}
