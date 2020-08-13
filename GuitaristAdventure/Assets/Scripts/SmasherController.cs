using System.Collections;
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
        Investigating,
        Dead

    }

    
    
    Transform playerTransform;
    NavMeshAgent agent;

    [Header("Movement")]
    [SerializeField] bool bShouldPatrol = true;
    public MonsterState currentState = MonsterState.Patrolling;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform currentWaypoint;
    int waypointIndex; 
    [SerializeField] int idlePercentChance = 80;
    [SerializeField] float minIdleTime, maxIdleTime;
    [SerializeField] SmasherAnimation smasherAnimation;
    bool bIdling = false;


    [Header("Combat")]
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask attackableLayers;
    [SerializeField] float attackRange;
    [SerializeField] float minAttackRate;
    [SerializeField] float maxAttackRate;
    [SerializeField] int attackDamage;
    [SerializeField] float minReactionTime;
    [SerializeField] float maxReactionTime;
    bool bAttacking = false;
    bool bReacting = false;
    float lastFireTime = 0f;
    float attackRate = 1f;

    
    

    [Header("Awareness")]
    [SerializeField] float defaultLookDistance = 10f;
    [SerializeField] float suspicionTime = 4f;
    [SerializeField] float suspicionLookDistance = 40f;
    [SerializeField] LayerMask sightLayers;
    [SerializeField] Transform headTransform;
    Vector3 lastKnownPlayerPosition;
    float lastPlayerAppearance = 0f;
    float investigationStartTime = 0f;



    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] bool bJumping = false;
    [SerializeField] bool bGrounded = false;
    Rigidbody rb;
    [SerializeField] float jumpMultiplier;
    CustomGravity customGravity;
    const float groundedRadius = .2f;
    float lastJumpTime = 0f;
    bool bEnteredJumpTrigger = false;
    JumpObject currentJumpObject;
    Transform[] currentJumpPoints;
    float[] currentJumpInfo;



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
        rb = GetComponent<Rigidbody>();
        customGravity = GetComponent<CustomGravity>();
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
                Rigidbody rigidbody = c.gameObject.GetComponent<Rigidbody>();
                
                ragdollParts.Add(c);
            }
        }

    }

    public void TurnOnRagdoll()
    {
        customGravity.bEnabled = false;
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
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.attachedRigidbody.AddForce(-transform.forward * 7f, ForceMode.Impulse);
        }
    }



    private void FixedUpdate()
    {
        bool bWasGrounded = bGrounded;
        bGrounded = false;

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundedRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                bGrounded = true;
                if (bWasGrounded == false)
                {
                        //landed on the ground
                        EndJump();
                    
                    

                }
                break;
            }
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
                    Debug.DrawRay(headTransform.position, playerTransform.position - headTransform.position, Color.white);
                    //if we're not facing the player turn to face the player 
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
                    }
                    if (bEnteredJumpTrigger && !bJumping && Time.time - lastJumpTime > 3f)
                    {
                        if (currentJumpObject)
                        {
                            StartJump(currentJumpPoints, currentJumpInfo,playerTransform.position);
                        }
                    }

                    if (distance < 3f)
                    {
                        if (Physics.CheckSphere(attackPoint.position,attackRange,attackableLayers))
                        {
                            
                            if (!bAttacking)
                            {
                                StartCoroutine("AttackRoutine",-1);
                            }
                        }
                    }

                    //Shoot
                    //if (Time.time - lastFireTime > fireRate)
                    //{

                    //    ShootWeapon();
                    //    fireRate = Random.Range(fireRateMin, fireRateMax);
                    //    lastFireTime = Time.time;
                    //}

                }
                //If we can't see the player, then stop chasing.
                if (bCanSeePlayer == false)
                {
                    lastPlayerAppearance = Time.time;
                    lastKnownPlayerPosition = playerTransform.position;
                    investigationStartTime = Time.time;
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
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
    }

    IEnumerator AttackRoutine(float rate = -1f)
    {

        bAttacking = true;
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
        if (rate != -1)
        {
            yield return new WaitForSeconds(rate);
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(minAttackRate, maxAttackRate));
        }
        smasherAnimation.Attack();
        yield return new WaitForSeconds(.25f);
        Debug.Log("attacked");
        if (Physics.CheckSphere(attackPoint.position, attackRange, attackableLayers))
        {
            Collider[] attackedColliders = Physics.OverlapSphere(attackPoint.position, attackRange, attackableLayers);
            foreach (Collider c in attackedColliders)
            {
                PlayerHealthManager playerHealth = c.gameObject.GetComponentInParent<PlayerHealthManager>();
                if (playerHealth)
                {
                    Debug.Log("Hurt player");
                    playerHealth.TakeDamage(attackDamage);
                    break;
                }
            }
        }
        if (currentState != MonsterState.Idling)
        {
            if (agent.enabled)
            {
                agent.isStopped = false;
            }
        }
        bAttacking = false;
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
        if (currentState == MonsterState.Investigating)
        {
            Gizmos.DrawWireSphere(lastKnownPlayerPosition, .3f);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SmasherJump"))
        {
            
            currentJumpObject = other.gameObject.GetComponentInParent<JumpObject>();
            currentJumpPoints = currentJumpObject.GetJumpPoints(groundCheck);
            currentJumpInfo = currentJumpObject.GetJumpInfo(groundCheck);
            bEnteredJumpTrigger = true;
            if (!bJumping)
            {
                if (currentState == MonsterState.Chasing)
                {

                    StartJump(currentJumpPoints, currentJumpInfo,playerTransform.position);
                    
                }
                else if (currentState == MonsterState.Patrolling)
                {
                    if (currentWaypoint)
                    {
                        float point2Distance = Vector3.SqrMagnitude(currentWaypoint.position - currentJumpPoints[1].position);
                        float point1Distance = Vector3.SqrMagnitude(currentWaypoint.position - currentJumpPoints[0].position);
                        //Check if jump would bring us closer to the current waypoint
                        if (point2Distance < point1Distance)
                        {
                            StartJump(currentJumpPoints, currentJumpInfo,currentWaypoint.position);
                        }
                    }
                }
                else if (currentState == MonsterState.Investigating)
                {
                    
                    //Check if jump would bring us closer to the player's last known position
                    float point2Distance = Vector3.SqrMagnitude(lastKnownPlayerPosition - currentJumpPoints[1].position);
                    float point1Distance = Vector3.SqrMagnitude(lastKnownPlayerPosition - currentJumpPoints[0].position);
                    if (point2Distance < point1Distance)
                    {
                        StartJump(currentJumpPoints, currentJumpInfo,lastKnownPlayerPosition);
                    }
                    else
                    {
                        Debug.Log("Jump not worth it");
                    }
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SmasherJump"))
        {
            bEnteredJumpTrigger = false;
        }
    }

    void StartJump(Transform[] jumpPoints, float[] jumpInfo,Vector3 target)
    {
        //only jump if we are jumping towards the player
        if (((jumpPoints[0].position - jumpPoints[1].position).z >= 0 && (transform.position - target).z >= 0)
            || ((jumpPoints[0].position - jumpPoints[1].position).z < 0 && (transform.position - target).z < 0))
        {
            float jumpPoint1TargetDif = Mathf.Abs(jumpPoints[0].position.y - target.y);
            float jumpPoint2TargetDif = Mathf.Abs(jumpPoints[1].position.y - target.y);
            //ensure that we are not jumping onto a platform above the player instead of just following the player
            if (jumpPoint2TargetDif <= jumpPoint1TargetDif)
            {
                lastJumpTime = Time.time;
                agent.enabled = false;
                bJumping = true;
                rb.isKinematic = false;
                Vector3 force = calcBallisticVelocityVector(groundCheck.position, jumpPoints[1].position, jumpInfo[0]);
                rb.AddForce(force * jumpInfo[1] * jumpMultiplier, ForceMode.VelocityChange);
            }
        }





    }

    void EndJump()
    {
        
        bJumping = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        agent.enabled = true;
    }


    //this method copied from https://answers.unity.com/questions/1362266/calculate-force-needed-to-reach-certain-point-addf-1.html
    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * customGravity.gravityScale / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }


}
