using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    
    
    [SerializeField] private float jumpForce = 400f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [Range(0, 1f)] [SerializeField] private float airControlPercentage = 0.5f;
    [SerializeField] Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;                           
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] bool bGrounded = false;
    [SerializeField] bool bAirControl = false;
    bool bFacingRight = true;
    Vector3 currentVelocity = Vector3.zero;
    const float groundedRadius = .2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                if (!bWasGrounded)
                {
                    //landed on the ground
                }
            }
        }

    }

    public void Move(float move, bool jump)
    {
        if (bGrounded || bAirControl)
        {
            if (!bGrounded && bAirControl)
            {
                //only allow deccelaration in air
                if ((move >= 0 && rb.velocity.z <= 0) || (move <= 0 && rb.velocity.z >= 0))
                {
                    move *= airControlPercentage;
                    rb.AddForce(new Vector3(0, 0, move * 20f));
                }
            }
            else
            {
                Vector3 targetVelocity = new Vector3(0, rb.velocity.y, move * 10f);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, movementSmoothing);
                
            }

        }

        if (bGrounded && jump)
        {
            bGrounded = false;
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }

        if (move > 0 && !bFacingRight)
        {
            Flip();
        }
        else if (move < 0 && bFacingRight)
        {
            Flip();
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

}
