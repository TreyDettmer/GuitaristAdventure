using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    
    
    
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    
    [SerializeField] Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;                           
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] bool bGrounded = false;
    [SerializeField] GameObject guitar;
    

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] bool bAirControl = false;
    [Range(0, 1f)] [SerializeField] private float airControlPercentage = 0.5f;
    CustomGravity customGravity;

    [Header("Ragdoll")]
    [SerializeField] List<Collider> ragdollParts = new List<Collider>();
    [SerializeField] Animator animator;

    bool bFacingRight = true;
    Vector3 currentVelocity = Vector3.zero;
    const float groundedRadius = .2f;
    public bool movementEnabled = true;

    private void Awake()
    {
        SetRagdollParts();
    }

    // Start is called before the first frame update
    void Start()
    {
        customGravity = GetComponent<CustomGravity>();
    }

    // Update is called once per frame
    void Update()
    {
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

        Destroy(guitar);
        customGravity.bEnabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;


        animator.enabled = false;
        animator.avatar = null;

        foreach (Collider c in ragdollParts)
        {
            c.enabled = true;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
            
        }
    }

    private void FixedUpdate()
    {
        bool bWasGrounded = bGrounded;
        bGrounded = false;
        
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundedRadius, groundLayer);
        for (int i = 0; i < colliders.Length;i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                bGrounded = true;
                if (bWasGrounded == false)
                {
                    //landed on the ground
                    playerAnimation.PlayerLanded();

                }
                break;
            }
        }

    }

    public void Move(float move, bool jump)
    {
        if (bGrounded || bAirControl)
        {
            if (!bGrounded && bAirControl)
            {
                if (movementEnabled)
                {
                    //only allow deccelaration in air
                    if ((move >= 0 && rb.velocity.z <= 0) || (move <= 0 && rb.velocity.z >= 0))
                    {
                        move *= airControlPercentage;
                        rb.AddForce(new Vector3(0, 0, move * 20f));
                    }
                }
            }
            else
            {
                if (movementEnabled)
                {
                    Vector3 targetVelocity = new Vector3(0, rb.velocity.y, move * 10f);
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, movementSmoothing);
                }
                
            }

        }

        if (bGrounded && jump)
        {
            //bGrounded = false;
            rb.AddForce(new Vector3(0, jumpForce, 0));
            playerAnimation.PlayerJumped();
        }

        if (move > 0 && !bFacingRight)
        {
            Flip();
        }
        else if (move < 0 && bFacingRight)
        {
            Flip();
        }
        if (move != 0 && bGrounded)
        {
            playerCombat.StopSerenading();
        }
        
    }

    void Flip()
    {
        bFacingRight = !bFacingRight;
        transform.Rotate(0, 180, 0);
        
    }

    public bool GetbGrounded()
    {
        return bGrounded;
    }

    private void OnTriggerEnter(Collider other)
    {
        DeathBarrier deathBarrier = other.gameObject.GetComponent<DeathBarrier>();
        if (deathBarrier)
        {
            deathBarrier.Kill(gameObject);
        }
    }

}
