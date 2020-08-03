using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public float waypointRadius = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        foreach (Transform child in transform)
        {
            Gizmos.DrawWireSphere(child.transform.position, waypointRadius);
        }
    }
}
