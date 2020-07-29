using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    [SerializeField]Rigidbody rb;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerController playerController;
    [SerializeField] Animator animator;
    [SerializeField] GuitarController guitar;

    float speedPercentage = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.GetbGrounded())
        {
            speedPercentage = Mathf.Clamp(Mathf.Abs(rb.velocity.z) / (.175f * playerMovement.GetMovementSpeed()), 0.0f, 1.0f);
        }
        else
        {
            speedPercentage = 0.0f;
        }
        animator.SetFloat("SpeedPercent", speedPercentage);

        if (rb.velocity.y < 0 && !playerController.GetbGrounded())
        {
            animator.SetBool("Falling", true);
        }
    }

    public void PlayerLanded()
    {
        Debug.Log("Setting grounded to true");
        animator.SetBool("Falling", false);
        animator.SetBool("Grounded", true);
    }

    public void PlayerJumped()
    {
        Debug.Log("Setting grounded to false");
        guitar.Jump();
        animator.SetBool("Grounded", false);
    }
}
