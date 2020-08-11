using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{


    public Transform[] jumpPoints;
    public float gizmoRadius;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (Transform point in jumpPoints)
        {
            Gizmos.DrawWireSphere(point.position, gizmoRadius);
        }
        

    }

    public Transform[] GetJumpPoints(Transform input)
    {
        
        float point1Distance = Vector3.SqrMagnitude(input.position - jumpPoints[0].position);
        float point2Distance = Vector3.SqrMagnitude(input.position - jumpPoints[1].position);
        if (point1Distance < point2Distance)
        {
            
            return new Transform[] { jumpPoints[0], jumpPoints[1] };
        }
        else
        {
            return new Transform[] { jumpPoints[1], jumpPoints[0] };
        }
    }
}
