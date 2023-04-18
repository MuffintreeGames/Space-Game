using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    private Rigidbody2D avatarRb;
    private Rigidbody2D goliathRb;

    private float offTrackMaxSpeed = 13f;
    private float offTrackMinSpeed = 5f;
    private float offTrackCurrentSpeed = 5f;
    private float offTrackAcceleration = 6f;

    private Vector2 assumedGoliathPosition; //used so that the avatar doesn't instantly react to goliath position changes; takes some time
    private float goliathPositionChangeMaxSpeed = 10f;
    private float goliathPositionChangeAcceleration = 3f;
    private float goliathPositionChangeSpeed = 0f;

    private Vector2 targetPosition;
    private bool targetPositionInitialized = false;
    private float patternRunTime = 0f;

    private float circleTime = 20f;
    private float circlingDistance = 10f;
    private float circleTolerance = 2f;
    // Start is called before the first frame update
    void Start()
    {
        avatarRb = GetComponent<Rigidbody2D>();
        goliathRb = GameObject.Find("Goliath").GetComponent<Rigidbody2D>();
        assumedGoliathPosition = goliathRb.position;
        //targetPosition = new Vector2(goliathRb.position.x, goliathRb.position.y + circlingDistance);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateGoliathPosition();
        PerformMovementPattern1();
        ApplyMovement();
    }

    void CalculateGoliathPosition()
    {
        float speedForFrame = Time.deltaTime * goliathPositionChangeSpeed;
        bool increaseSpeed = false;

        if (Mathf.Abs(assumedGoliathPosition.x - goliathRb.position.x) <= speedForFrame)
        {
            assumedGoliathPosition.x = goliathRb.position.x;
        }
        else if (goliathRb.position.x > assumedGoliathPosition.x)
        {
            assumedGoliathPosition.x += speedForFrame;
            increaseSpeed = true;
        }
        else
        {
            assumedGoliathPosition.x -= speedForFrame;
            increaseSpeed = true;
        }

        if (Mathf.Abs(assumedGoliathPosition.y - goliathRb.position.y) <= speedForFrame)
        {
            assumedGoliathPosition.y = goliathRb.position.y;
        }
        else if (goliathRb.position.y > assumedGoliathPosition.y)
        {
            assumedGoliathPosition.y += speedForFrame;
            increaseSpeed = true;
        }
        else
        {
            assumedGoliathPosition.y -= speedForFrame;
            increaseSpeed = true;
        }

        if (increaseSpeed)
        {
            goliathPositionChangeSpeed += goliathPositionChangeAcceleration * Time.deltaTime;
            goliathPositionChangeSpeed = Mathf.Min(goliathPositionChangeSpeed, goliathPositionChangeMaxSpeed);
        }
        else
        {
            goliathPositionChangeSpeed -= goliathPositionChangeAcceleration * Time.deltaTime;
            goliathPositionChangeSpeed = Mathf.Min(goliathPositionChangeSpeed, 0f);
        }
    }

    void PerformMovementPattern1()  //circle around goliath
    {
        patternRunTime %= circleTime;
        Vector3 rotationAxis = new Vector3(0, 0, 1);
        Quaternion q = Quaternion.AngleAxis(360f * (patternRunTime / circleTime), rotationAxis);
        Vector2 DefaultDistance = new Vector2(circlingDistance, circlingDistance);
        Vector2 calculatedPosition = (Vector2) (q * DefaultDistance) + assumedGoliathPosition;
        Debug.Log("calculated: " + calculatedPosition + ", current: " + avatarRb.position);
        if (!targetPositionInitialized)
        {
            targetPosition = calculatedPosition;
            targetPositionInitialized = true;
            return;
        }

        if (Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance || Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance)
        {
            //Debug.Log("avatar is off-track, pausing pattern");
        }
        else
        {
            targetPosition = calculatedPosition;
            patternRunTime += Time.deltaTime;
        }
    }

    void ApplyMovement()
    {
        Vector2 newPosition = avatarRb.position;
        float speedForFrame = Time.deltaTime * offTrackCurrentSpeed;
        bool increaseSpeed = false;

        if (Mathf.Abs(targetPosition.x - avatarRb.position.x) <= speedForFrame)
        {
            newPosition.x = targetPosition.x;
        } else if (targetPosition.x > avatarRb.position.x)
        {
            newPosition.x += speedForFrame;
            increaseSpeed = true;
        } else
        {
            newPosition.x -= speedForFrame;
            increaseSpeed = true;
        }

        if (Mathf.Abs(targetPosition.y - avatarRb.position.y) <= speedForFrame)
        {
            newPosition.y = targetPosition.y;
        }
        else if (targetPosition.y > avatarRb.position.y)
        {
            newPosition.y += speedForFrame;
            increaseSpeed = true;
        }
        else
        {
            newPosition.y -= speedForFrame;
            increaseSpeed=true;
        }

        avatarRb.MovePosition(newPosition);

        if (increaseSpeed)
        {
            offTrackCurrentSpeed += offTrackAcceleration * Time.deltaTime;
            offTrackCurrentSpeed = Mathf.Min(offTrackCurrentSpeed, offTrackMaxSpeed);
            //Debug.Log("speeding up avatar");
        } else
        {
            offTrackCurrentSpeed -= offTrackAcceleration * Time.deltaTime;
            offTrackCurrentSpeed = Mathf.Max(offTrackCurrentSpeed, offTrackMinSpeed);
        }
    }
}
