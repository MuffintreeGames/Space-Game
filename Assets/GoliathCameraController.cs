using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathCameraController : MonoBehaviour
{
    public Transform target;    //goliath transform, point that camera tries to stay locked onto
    public float smoothSpeed = 0.25f;  //speed that camera should go at while target is within smooth follow distance

    public float smoothFollowHorDistance = 4f;  //if goliath is more than this distance away horizontally, slowly pan towards them
    public float hardFollowHorDistance = 8f;    //if goliath is more than this distance away, hard lock-on to them

    public float smoothFollowVertDistance = 2f; //as above, but vertical
    public float hardFollowVertDistance = 4f;

    private float targetZoom = 8f;  //zoom distance on the camera. Increases as goliath grows
    private float zoomSpeed = 0.5f;   //speed at which camera zooms out

    private Camera thisCamera;

    public void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    public void updateCamera()
    {
        thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
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

    public void SetZoom(float zoomLevel)
    {
        targetZoom = zoomLevel;
    }
}
