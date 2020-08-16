using UnityEngine;

/*
 * Credit for this script goes to the user Edy on the Unity forums. The script
 * can be found here https://forum.unity.com/threads/why-does-rigidbody-3d-not-have-a-gravity-scale.440415/ 
 */


[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    // Gravity Scale editable on the inspector
    // providing a gravity scale per object

    public float gravityScale = 1.0f;
    public float timeScale = 1.0f;
    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.

    public static float globalGravity = -9.81f;
    public bool bEnabled = true;

    Rigidbody m_rb;

    private void Start()
    {
        Time.timeScale = timeScale;
    }

    void OnEnable()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (bEnabled)
        {
            Vector3 gravity = -gravityScale * Vector3.up;
            m_rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }


}