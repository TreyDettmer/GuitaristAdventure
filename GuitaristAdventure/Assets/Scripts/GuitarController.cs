using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform backTransform;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform swingPointTransform;
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
        animator.SetTrigger("Jump");
    }

    public void Swing()
    {
        SetIntoSwingPosition();
        animator.SetTrigger("Swing");
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
        //transform.localPosition = new Vector3(1.092f, 0.404f, 2.142f);
        //transform.localEulerAngles = new Vector3(147.683f, -168.05f, -10.84299f);
        //transform.localScale = Vector3.one;
    }
}
