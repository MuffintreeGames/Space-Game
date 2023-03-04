using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathCameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.005f;

    public float smoothFollowHorDistance = 6f;
    public float hardFollowHorDistance = 8f;

    public float smoothFollowVertDistance = 3f;
    public float hardFollowVertDistance = 4f;

    public void updateCamera()
    {
        Vector3 distanceToTarget = target.position - transform.position;
        Vector3 smoothTarget = new Vector3(target.position.x, target.position.y, 0);
        float newXPosition = transform.position.x;
        float newYPosition = transform.position.y;
        if (distanceToTarget.x > hardFollowHorDistance)
        {
            newXPosition = target.position.x - hardFollowHorDistance;
        }
        else if (distanceToTarget.x < -hardFollowHorDistance)
        {
            newXPosition = target.position.x + hardFollowHorDistance;
        } else if (distanceToTarget.x > smoothFollowHorDistance) {
            smoothTarget.x -= smoothFollowHorDistance;
            Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed);
            newXPosition = smoothCameraVector.x;
        } else if (distanceToTarget.y < -smoothFollowHorDistance)
        {
            smoothTarget.x += smoothFollowHorDistance;
            Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed);
            newXPosition = smoothCameraVector.x;
        }

        if (distanceToTarget.y > hardFollowVertDistance)
        {
            newYPosition = target.position.y - hardFollowVertDistance;
        }
        else if (distanceToTarget.y < -hardFollowVertDistance)
        {
            newYPosition = target.position.y + hardFollowVertDistance;
        }
        else if (distanceToTarget.y > smoothFollowVertDistance)
        {
            smoothTarget.y -= smoothFollowVertDistance;
            Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed);
            newYPosition = smoothCameraVector.y;
        }
        else if (distanceToTarget.y < -smoothFollowVertDistance)
        {
            smoothTarget.y += smoothFollowVertDistance;
            Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed);
            newYPosition = smoothCameraVector.y;
        }

        transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
        distanceToTarget = target.position - transform.position;
    }
}
