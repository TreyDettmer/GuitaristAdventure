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
        Idling,
        Investigating,
        Dead
        
    }

    
    
    Transform playerTransform;
    NavMeshAgent agent;

    [Header("Movement")]
    [SerializeField] bool bShouldPatrol = false;
    public MonsterState currentState = MonsterState.Patrolling;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform currentWaypoint;
    int waypointIndex;
    [SerializeField] int idlePercentChance = 80;
    [SerializeField] float minIdleTime, maxIdleTime;
    bool bIdling = false;
    


    
    [Header("Combat")]
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponTip;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform weaponCurrentLookAtTransform;
    [SerializeField] float fireRateMin, fireRateMax;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] float minReactionTime;
    [SerializeField] float maxReactionTime;
    bool bReacting = false;
    float lastFireTime = 0f;
    float fireRate = 1f;
    Vector3 weaponDefaultLookAt;

    [Header("Awareness")]
    [SerializeField] float defaultLookDistance = 10f;
    [SerializeField] float suspicionTime = 4f;
    [SerializeField] float suspicionLookDistance = 40f;
    [SerializeField] LayerMask sightLayers;
    [SerializeField] Transform headTransform;
    Vector3 lastKnownPlayerPosition;
    float lastPlayerAppearance = 0f;
    float investigationStartTime = 0f;


    [Header("Ragdoll")]
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
        weaponDefaultLookAt = weaponCurrentLookAtTransform.localPosition;
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
        
        //this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        weapon.GetComponent<BoxCollider>().enabled = true;
        weapon.GetComponent<Rigidbody>().useGravity = true;
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
        if (currentState == MonsterState.Dead)
        {
            return;
        }
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool bCanSeePlayer = false;
        RaycastHit hit;

        if (currentState == MonsterState.Idling || currentState == MonsterState.Patrolling)
        {
            if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, defaultLookDistance, sightLayers))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    bCanSeePlayer = true;
                }
            }
        }
        else
        {
            if (Physics.Raycast(headTransform.position, playerTransform.position - headTransform.position, out hit, suspicionLookDistance, sightLayers))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    bCanSeePlayer = true;
                }
            }
        }
        switch (currentState)
        {
            case MonsterState.Chasing:

                if (bCanSeePlayer)
                {
                    //Continue to chase
                    if (Vector3.Angle(playerTransform.position - transform.position, transform.forward) >= 90)
                    {
                        if (!bReacting)
                        {
                            //not turning instantly gives the player time to attack from behind
                            StartCoroutine("ReactionDelayRoutine");
                        }
                        
                    }
                    else
                    {
                        if (agent.enabled && !bReacting)
                        {
                            agent.SetDestination(playerTransform.position);
                        }
                        //don't point gun at exact player location when player is close. Doesn't look good.
                        if (distance > 2.5f)
                        {
                            weaponCurrentLookAtTransform.position = playerTransform.position;
                        }
                        //Shoot
                        if (Time.time - lastFireTime > fireRate)
                        {

                            ShootWeapon();
                            fireRate = Random.Range(fireRateMin, fireRateMax);
                            lastFireTime = Time.time;
                        }
                    }


                }
                //If we can't see the player, then stop chasing.
                if (bCanSeePlayer == false)
                {
                    lastPlayerAppearance = Time.time;
                    lastKnownPlayerPosition = playerTransform.position;
                    investigationStartTime = Time.time;
                    weaponCurrentLookAtTransform.localPosition = weaponDefaultLookAt;
                    currentState = MonsterState.Investigating;

                }

                break;
            case MonsterState.Investigating:
                if (Time.time - investigationStartTime <= suspicionTime)
                {
                    if (agent.enabled)
                    {
                        agent.SetDestination(lastKnownPlayerPosition);

                    }
                    if (bCanSeePlayer)
                    {
                        currentState = MonsterState.Chasing;
                    }
                }
                else
                {
                    currentState = MonsterState.Idling;
                    Idle();
                }
                break;


            case MonsterState.Idling:
                if (bIdling == false)
                {
                    Idle();

                }
                //If we can see the player, then start chasing
                if (bCanSeePlayer)
                {
                    currentState = MonsterState.Chasing;
                    StopCoroutine("IdleRoutine");
                    if (agent.enabled)
                    {
                        agent.isStopped = false;
                    }
                    
                    bIdling = false;
                }

                

                 break;
            case MonsterState.Patrolling:
                if (bShouldPatrol)
                {
                    if (currentWaypoint)
                    {
                        if (agent.enabled)
                        {
                            //Check if we have reached the current waypoint
                            if (agent.remainingDistance < 2f)
                            {
                                //Go to the next waypoint
                                UpdateWaypoint();
                            }
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

        Rigidbody bulletRb;
        bulletRb = Instantiate(bullet, weaponTip.position, Quaternion.identity).GetComponent<Rigidbody>();
        bulletRb.AddForce((playerTransform.position + new Vector3(0,2f,0) - headTransform.position).normalized * bulletSpeed, ForceMode.Impulse);
        
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

    IEnumerator ReactionDelayRoutine()
    {
        bReacting = true;
        yield return new WaitForSeconds(Random.Range(minReactionTime, maxReactionTime));
        if (currentState != MonsterState.Dead)
        {
            FaceTarget(playerTransform);
            if (agent.enabled)
            {
                agent.SetDestination(playerTransform.position);
            }
            bReacting = false;
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
