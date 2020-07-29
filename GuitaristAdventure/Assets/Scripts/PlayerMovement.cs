using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private PlayerController controller;
    [SerializeField] Rigidbody rb;
    [SerializeField] private float movementSpeed = 10f;
    float horizontalMove = 0f;
    bool jump = false;

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * movementSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }


    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }

    public float GetMovementSpeed() { return movementSpeed; }



}
