using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform backTransform;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform swingPointTransform;
    [SerializeField] Transform serenadePointTransform;
    // Start is called before the first frame update
    void Start()
    {
        AttachToBack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool bActive)
    {
        AttachToBack();
        gameObject.SetActive(bActive);
    }

    public void Jump()
    {
        SetIntoJumpPosition();
        gameObject.SetActive(true);
        animator.SetBool("Serenading", false);
        animator.SetTrigger("Jump");
    }

    public void Swing()
    {
        SetIntoSwingPosition();
        animator.SetBool("Serenading", false);
        animator.SetTrigger("Swing");
    }

    public void Serenade()
    {
        SetIntoSerenadePosition();
        animator.SetBool("Serenading", true);
    }

    public void StopSerenading()
    {
        animator.SetBool("Serenading", false);
        AttachToBack();
    }

    public void Disable()
    {
        AttachToBack();
        gameObject.SetActive(false);
    }


    public void AttachToBack()
    {
        transform.SetParent(backTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0,0,0,1);
        transform.localScale = Vector3.one;
    }

    void SetIntoJumpPosition()
    {
        transform.SetParent(playerTransform);
        transform.localPosition = new Vector3(.012f, -0.783f, -0.122f);
        transform.localRotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = Vector3.one;
    }

    void SetIntoSwingPosition()
    {
        transform.SetParent(swingPointTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0,0,0,1);
        transform.localScale = Vector3.one;
    }

    void SetIntoSerenadePosition()
    {
        transform.SetParent(serenadePointTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = Vector3.one;
    }
}
