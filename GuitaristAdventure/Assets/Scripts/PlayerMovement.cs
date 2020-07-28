using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private PlayerController controller;
    [SerializeField] Rigidbody rb;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] Animator animator;
    float horizontalMove = 0f;
    bool jump = false;
    float speedPercentage = 0.0f;

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * movementSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (controller.GetbGrounded())
        {
            speedPercentage = Mathf.Clamp(Mathf.Abs(rb.velocity.z) / (.175f * movementSpeed), 0.0f, 1.0f);
        }
        else
        {
            speedPercentage = 0.0f;
        }
        animator.SetFloat("SpeedPercent", speedPercentage);

    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }


}
