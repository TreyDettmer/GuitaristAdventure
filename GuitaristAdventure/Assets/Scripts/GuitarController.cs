using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform backTransform;
    [SerializeField] Transform playerTransform;
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

    public void Disable()
    {
        AttachToBack();
        gameObject.SetActive(false);
    }


    void AttachToBack()
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
}
