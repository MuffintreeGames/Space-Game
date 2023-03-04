using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathController : MonoBehaviour
{
    private GameObject goliath;
    private Transform goliathTransform;
    private float maxSpeed = 7.5f;
    private float acceleration = 1.5f;
    private float reversingAcceleration = 4f;
    private float deceleration = 2f;
    private float currentHorSpeed = 0f;
    private float currentVertSpeed = 0f;
    private float rotationSpeed = 0.2f;

    public GoliathCameraController goliathCamera;

    // Start is called before the first frame update
    void Start()
    {
        goliath = this.gameObject;
        goliathTransform = goliath.transform;
    }

    void SetGoliathRotation()
    {
        float targetRotation = goliathTransform.eulerAngles.z;
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");
        if (horizontalDirection > 0f)
        {
        } else if (horizontalDirection < 0f)
        {
        }

        if (horizontalDirection == 0f && verticalDirection == 0f)
        {
            return;
        }

        if (horizontalDirection > 0f)   //moving right
        {
            if (verticalDirection > 0f)
            {
                targetRotation = 315f;
            }
            else if (verticalDirection < 0f)
            {
                targetRotation = 225f;
            }
            else
            {
                targetRotation = 270f;
            }
        }
        else if (horizontalDirection < 0f)  //moving left
        {
            if (verticalDirection > 0f)
            {
                targetRotation = 45f;
            }
            else if (verticalDirection < 0f)
            {
                targetRotation = 135f;
            }
            else
            {
                targetRotation = 90f;
            }
        } else {    //moving vertically
            if (verticalDirection > 0f)
            {
                targetRotation = 0f;
            }
            else if (verticalDirection < 0f)
            {
                targetRotation = 180f;
            }
        }

        if (targetRotation - goliathTransform.eulerAngles.z > 180f) //ensure we're rotating in the fastest direction
        {
            targetRotation = -(360f - targetRotation);
        }

        float smoothedRotation = Mathf.Lerp(goliathTransform.eulerAngles.z, targetRotation, rotationSpeed);
        transform.eulerAngles = new Vector3(goliathTransform.eulerAngles.x, goliathTransform.eulerAngles.y, smoothedRotation);
    }

    void SetGoliathSpeed()  //apply acceleration/deceleration based on input
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");
        float adjustedAccel = acceleration * Time.deltaTime;
        float adjustedDecel = deceleration * Time.deltaTime;
        float adjustedReversingAccel = reversingAcceleration * Time.deltaTime;

        if (horizontalDirection > 0f)   //set horizontal speed
        {
            if (currentHorSpeed < 0f)   //accelerate faster when going the opposite direction to reach a standstill sooner
            {
                currentHorSpeed += adjustedReversingAccel;
            }
            else
            {
                currentHorSpeed += adjustedAccel;
            }
            currentHorSpeed = Mathf.Min(maxSpeed, currentHorSpeed);
        }
        else if (horizontalDirection < 0f)
        {
            if (currentHorSpeed > 0f)
            {
                currentHorSpeed -= adjustedReversingAccel;
            } else
            {
                currentHorSpeed -= adjustedAccel;
            }
            currentHorSpeed = Mathf.Max(-maxSpeed, currentHorSpeed);
        }
        else if (currentHorSpeed > 0f)
        {
            currentHorSpeed -= adjustedDecel;
            currentHorSpeed = Mathf.Max(0f, currentHorSpeed);
        }
        else if (currentHorSpeed < 0f)
        {
            currentHorSpeed += adjustedDecel;
            currentHorSpeed = Mathf.Min(0f, currentHorSpeed);
        }


        if (verticalDirection > 0f) //set vertical speed
        {
            if (currentVertSpeed < 0f) {
                currentVertSpeed += adjustedReversingAccel;
            } else
            {
                currentVertSpeed += adjustedAccel;
            }
            currentVertSpeed = Mathf.Min(maxSpeed, currentVertSpeed);
        }
        else if (verticalDirection < 0f)
        {
            if (currentVertSpeed > 0f) {
                currentVertSpeed -= adjustedReversingAccel;
            } else
            {
                currentVertSpeed -= adjustedAccel;
            }
            currentVertSpeed = Mathf.Max(-maxSpeed, currentVertSpeed);
        }
        else if (currentVertSpeed > 0f)
        {
            currentVertSpeed -= adjustedDecel;
            currentVertSpeed = Mathf.Max(0f, currentVertSpeed);
        }
        else if (currentVertSpeed < 0f)
        {
            currentVertSpeed += adjustedDecel;
            currentVertSpeed = Mathf.Min(0f, currentVertSpeed);
        }
    }

    void MoveGoliath()  //apply movement based off of current speed settings
    {
        goliathTransform.position = goliathTransform.position + new Vector3(currentHorSpeed * Time.deltaTime, currentVertSpeed * Time.deltaTime, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SetGoliathSpeed();
        SetGoliathRotation();
        MoveGoliath();
        goliathCamera.updateCamera();   //now that we've moved, set new camera position
    }
}
