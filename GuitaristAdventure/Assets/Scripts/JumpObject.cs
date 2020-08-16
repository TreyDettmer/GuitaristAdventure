using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class JumpObject : MonoBehaviour
{


    public Transform[] jumpPoints;
    public float gizmoRadius;
    public float gravity = 29.43f;
    public float jump12Height;
    public float jump21Height;
    public bool debugPath;



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (Transform point in jumpPoints)
        {
            Gizmos.DrawWireSphere(point.position, gizmoRadius);
        }
        

    }

    public int GetJumpPositionIndex(Transform input)
    {
        
        float point1Distance = Vector3.SqrMagnitude(input.position - jumpPoints[0].position);
        float point2Distance = Vector3.SqrMagnitude(input.position - jumpPoints[1].position);
        if (point1Distance < point2Distance)
        {

            return 0;
        }
        else
        {
            return 1;
        }
    }

    public Transform[] GetJumpPoints(int firstIndex = 0)
    {
        if (firstIndex == 0)
        {
            return new Transform[] { jumpPoints[0], jumpPoints[1] };
        }
        else
        {
            return new Transform[] { jumpPoints[1], jumpPoints[0] };
        }
    }



    public void Update()
    {
        if (debugPath)
        {
            DrawPath12();
            DrawPath21();

        }
    }


    public Vector3 Launch(Vector3 startPosition,int startingIndex = 0)
    {
        if (startingIndex == 0)
        {
            return CalculateLaunchData12(startPosition).initialVelocity;
        }
        else
        {
            return CalculateLaunchData21(startPosition).initialVelocity;
        }
    }


    LaunchData CalculateLaunchData21(Vector3 startPosition)
    {
        float displacementY = jumpPoints[0].position.y - startPosition.y;
        Vector3 displacementXZ = new Vector3(jumpPoints[0].position.x - startPosition.x, 0, jumpPoints[0].position.z - startPosition.z);
        float time = Mathf.Sqrt(-2 * jump21Height / -gravity) + Mathf.Sqrt(2 * (displacementY - jump21Height) / -gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * -gravity * jump21Height);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(-gravity), time);
    }



    LaunchData CalculateLaunchData12(Vector3 startPosition)
    {
        float displacementY = jumpPoints[1].position.y - startPosition.y;
        Vector3 displacementXZ = new Vector3(jumpPoints[1].position.x - startPosition.x, 0, jumpPoints[1].position.z - startPosition.z);
        float time = Mathf.Sqrt(-2 * jump12Height / -gravity) + Mathf.Sqrt(2 * (displacementY - jump12Height) / -gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * -gravity * jump12Height);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(-gravity), time);
    }


    void DrawPath12()
    {
        LaunchData launchData = CalculateLaunchData12(jumpPoints[0].position);
        Vector3 previousDrawPoint = jumpPoints[0].position;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * -gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = jumpPoints[0].position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }


    void DrawPath21()
    {
        LaunchData launchData = CalculateLaunchData21(jumpPoints[1].position);
        Vector3 previousDrawPoint = jumpPoints[1].position;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * -gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = jumpPoints[1].position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.red);
            previousDrawPoint = drawPoint;
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }

















}
