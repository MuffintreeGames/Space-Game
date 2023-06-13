using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathCameraController : MonoBehaviour
{
    public Transform goliathTarget;    //goliath transform, point that camera tries to stay locked onto normally
    public Transform freeTarget;    //invisible transform used in free camera mode
    private Transform target;
    public float smoothSpeed = 0.25f;  //speed that camera should go at while target is within smooth follow distance
    public float freeCameraSpeed = 800f;    //speed that camera should go at during free camera mode

    public float smoothFollowHorDistance = 4f;  //if goliath is more than this distance away horizontally, slowly pan towards them
    public float hardFollowHorDistance = 8f;    //if goliath is more than this distance away, hard lock-on to them

    public float smoothFollowVertDistance = 2f; //as above, but vertical
    public float hardFollowVertDistance = 4f;

    private float targetZoom = 12f;  //zoom distance on the camera. Increases as goliath grows
    private float godMultiplier = 2f;   //if playing as god, use god multiplier
    private float zoomSpeed = 0.5f;   //speed at which camera zooms out

    private float zoomMultiplier = 1f;

    private Camera thisCamera;
    private bool freeCameraMode = false;    //if true, god can move camera around freely

    public void Start()
    {
        thisCamera = GetComponent<Camera>();
        if (!RoleManager.isGoliath)
        {
            zoomMultiplier = godMultiplier;
        }
        target = goliathTarget;
    }

    public void Update()
    {
        updateCamera();
    }

    public void updateCamera()
    {
        thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, targetZoom * zoomMultiplier, zoomSpeed * zoomMultiplier * Time.deltaTime);
        if (!freeCameraMode)
        {
            if (target == null)
            {
                return;
            }

            Vector3 distanceToTarget = target.position - transform.position;
            Vector3 smoothTarget = new Vector3(target.position.x, target.position.y, 0);
            float newXPosition = transform.position.x;
            float newYPosition = transform.position.y;
            if (distanceToTarget.x > (hardFollowHorDistance * zoomMultiplier))
            {
                newXPosition = target.position.x - (hardFollowHorDistance * zoomMultiplier);
            }
            else if (distanceToTarget.x < (-hardFollowHorDistance * zoomMultiplier))
            {
                newXPosition = target.position.x + (hardFollowHorDistance * zoomMultiplier);
            }
            else if (distanceToTarget.x > (smoothFollowHorDistance * zoomMultiplier))
            {
                smoothTarget.x -= (smoothFollowHorDistance * zoomMultiplier);
                Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed * zoomMultiplier);
                newXPosition = smoothCameraVector.x;
            }
            else if (distanceToTarget.y < -(smoothFollowHorDistance * zoomMultiplier))
            {
                smoothTarget.x += (smoothFollowHorDistance * zoomMultiplier);
                Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed * zoomMultiplier);
                newXPosition = smoothCameraVector.x;
            }

            if (distanceToTarget.y > hardFollowVertDistance * zoomMultiplier)
            {
                newYPosition = target.position.y - (hardFollowVertDistance * zoomMultiplier);
            }
            else if (distanceToTarget.y < -hardFollowVertDistance * zoomMultiplier)
            {
                newYPosition = target.position.y + (hardFollowVertDistance * zoomMultiplier);
            }
            else if (distanceToTarget.y > smoothFollowVertDistance * zoomMultiplier)
            {
                smoothTarget.y -= smoothFollowVertDistance * zoomMultiplier;
                Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed * zoomMultiplier);
                newYPosition = smoothCameraVector.y;
            }
            else if (distanceToTarget.y < -smoothFollowVertDistance * zoomMultiplier)
            {
                smoothTarget.y += smoothFollowVertDistance * zoomMultiplier;
                Vector3 smoothCameraVector = Vector3.Lerp(transform.position, smoothTarget, smoothSpeed * zoomMultiplier);
                newYPosition = smoothCameraVector.y;
            }

            transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
            distanceToTarget = target.position - transform.position;
        } else
        {
            float horizontalDirection = Input.GetAxisRaw("Horizontal");
            float verticalDirection = Input.GetAxisRaw("Vertical");
            Vector2 transformVector = new Vector2(horizontalDirection * freeCameraSpeed * Time.deltaTime, verticalDirection * freeCameraSpeed * Time.deltaTime);
            transform.Translate(transformVector);
        }
    }

    public void SetZoom(float zoomLevel)
    {
        targetZoom = zoomLevel;
    }

    public void ToggleFollowMode()
    {
        if (freeCameraMode)
        {
            freeCameraMode = false;
            //target = goliathTarget;
        } else
        {
            freeCameraMode = true;
            //target = freeTarget;
        }
    }
}
