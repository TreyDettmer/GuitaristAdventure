using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class MonsterController : MonoBehaviour
{

    public enum MonsterState
    {
        Patrolling,
        Chasing,
        Idling
    }

    [SerializeField] bool bShouldPatrol = false;
    [SerializeField] float lookRadius = 10f;
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
    Vector3 weaponDefaultLookAt;
    [SerializeField] Transform weaponCurrentLookAtTransform;
    [SerializeField] LayerMask sightLayers;
    [SerializeField] Transform headTransform;
    [SerializeField] Transform weaponTip;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed = 20f;

    private void Start()
    {
        playerTransform = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        if (bShouldPatrol)
        {
            GetRandomWaypoint();
        }
        weaponDefaultLookAt = weaponCurrentLookAtTransform.localPosition;
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool bCanSeePlayer = false;
        if (distance <= lookRadius)
        {
            RaycastHit hit;
            if (Physics.Raycast(headTransform.position,playerTransform.position - headTransform.position, out hit,lookRadius,sightLayers))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    bCanSeePlayer = true;
                    if (currentState != MonsterState.Chasing)
                    {
                        if (currentState == MonsterState.Idling)
                        {
                            currentState = MonsterState.Chasing;
                            StopCoroutine("IdleRoutine");
                            agent.isStopped = false;
                            bIdling = false;

                        }
                        else
                        {
                            currentState = MonsterState.Chasing;
                        }

                    }
                    FaceTarget(playerTransform);
                    agent.SetDestination(playerTransform.position);
                }
                else
                {
                    if (currentState == MonsterState.Chasing)
                    {
                        weaponCurrentLookAtTransform.localPosition = weaponDefaultLookAt;
                        currentState = MonsterState.Idling;
                    }
                }
            }
            else
            {
                if (currentState == MonsterState.Chasing)
                {
                    weaponCurrentLookAtTransform.localPosition = weaponDefaultLookAt;
                    currentState = MonsterState.Idling;
                }
            }

        }
        else
        {
            if (currentState == MonsterState.Chasing)
            {
                weaponCurrentLookAtTransform.localPosition = weaponDefaultLookAt;
                currentState = MonsterState.Idling;
            }
        }
        switch (currentState)
        {
            case MonsterState.Chasing:
                if (distance > 2.5f && bCanSeePlayer)
                {
                    weaponCurrentLookAtTransform.position = playerTransform.position;
                }
                if (Time.time - lastFireTime > fireRate)
                {
                    
                    ShootWeapon();
                    fireRate = Random.Range(fireRateMin, fireRateMax);
                    lastFireTime = Time.time; 
                }
                break;
            case MonsterState.Idling:
                if (bIdling == false)
                {
                    Idle();
                    
                }
                break;
            case MonsterState.Patrolling:
                if (bShouldPatrol)
                {
                    if (currentWaypoint)
                    {

                        if (agent.remainingDistance < 2f)
                        {
                            UpdateWaypoint();
                        }
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

    void Idle()
    {
        bIdling = true;
        StartCoroutine("IdleRoutine",Random.Range(minIdleTime,maxIdleTime));
        agent.isStopped = true;
    }

    void ShootWeapon()
    {
        //Debug.Log("Bang");
        Rigidbody bulletRb;
        bulletRb = Instantiate(bullet, weaponTip.position, Quaternion.identity).GetComponent<Rigidbody>();
        bulletRb.AddForce((playerTransform.position + new Vector3(0,2f,0) - headTransform.position).normalized * bulletSpeed, ForceMode.Impulse);
        
    }
    

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
       

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
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
