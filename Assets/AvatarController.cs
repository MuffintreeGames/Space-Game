using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AvatarController : Killable    //script to manage AI for god avatar
{
    private Rigidbody2D avatarRb;
    private Rigidbody2D goliathRb;

    private float arenaSize = 50f;

    private int currentMovementPattern = -1;    //-1 means unset, 0 = circle clockwise, 1 = circle counter, 2 = short-range teleporting, 3 = hiding, 4 = running
    private int previousMovementPattern = 10;
    private float damageTakenInPattern = 0f;
    private float damageLimit = 100f;

    private float maxSpeed = 13f;
    private float minSpeed = 5f;
    private float currentSpeed = 5f;
    private float acceleration = 6f;

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
    private float circleDistanceCovered = 0f;

    private float hideMargin = 10f; //min distance allowed between avatar and wall

    private float blinkDistance = 15f;
    private float blinkVariance = 3f;
    private int totalBlinks = 5;
    private int currentBlinks = 0;
    private float timeBetweenBlinks = 5f;

    private float hideDistance = 30f;
    private float hideVariance = 5f;
    private float hideTime = 20f;
    // Start is called before the first frame update
    new void Start()
    {
        avatarRb = GetComponent<Rigidbody2D>();
        goliathRb = GameObject.Find("Goliath").GetComponent<Rigidbody2D>();
        assumedGoliathPosition = goliathRb.position;
        base.Start();
        //targetPosition = new Vector2(goliathRb.position.x, goliathRb.position.y + circlingDistance);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateGoliathPosition();
        bool movementDone = false;
        if (damageTakenInPattern >= damageLimit)    //took too much damage in current pattern; swapping
        {
            currentMovementPattern = -1;
        }
        while (!movementDone)
        {
            switch (currentMovementPattern)
            {
                case -1:
                    SelectMovementPattern(); break;
                case 0:
                    PerformMovementPatternCircleClockwise(); ApplyMovement(); movementDone = true; break;
                case 1:
                    PerformMovementPatternCircleCounterClockwise(); ApplyMovement(); movementDone = true; break;
                case 2:
                    PerformMovementPatternRapidBlinks(); movementDone = true; break;
                case 3:
                    PerformMovementPatternFlee(); ApplyMovement();  movementDone = true; break;
            }
        }
    }

    private void SelectMovementPattern()    //pick one of the 4 movement patterns, excluding the last one chosen
    {
        currentMovementPattern = Random.Range(0, 1);
        if (currentMovementPattern >= previousMovementPattern)
        {
            //currentMovementPattern += 1;
            if (currentMovementPattern > 3)
            {
                currentMovementPattern = 0;
            }
        }
        previousMovementPattern = currentMovementPattern;
        damageTakenInPattern = 0f;
        patternRunTime = 0f;
        targetPositionInitialized = false;
        Debug.Log("chosen: " + currentMovementPattern);
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

    void PerformMovementPatternCircleClockwise()  //circle around goliath clockwise
    {
        if (patternRunTime > circleTime)
        {
            currentMovementPattern = -1;
            circleDistanceCovered = 0;
            return;
        }

        Vector3 rotationAxis = new Vector3(0, 0, 1);

        float positionInCircle = circleDistanceCovered / (2 * circlingDistance * Mathf.PI); //circumference = 2*pi*r
        if (positionInCircle >= 1f)
        {
            positionInCircle -= 1f;
        }
        Quaternion q = Quaternion.AngleAxis(-360f * positionInCircle, rotationAxis);
        Vector2 DefaultDistance = new Vector2(circlingDistance, circlingDistance);
        Vector2 calculatedPosition = (Vector2) (q * DefaultDistance) + assumedGoliathPosition;
        if (!targetPositionInitialized) //warps avatar into place
        {
            targetPosition = calculatedPosition;
            avatarRb.position = calculatedPosition;
            targetPositionInitialized = true;
            return;
        }

        if (!(Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance || Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance)) //when avatar is too far off the path, should pause the pattern until they're back on track
        {
            circleDistanceCovered += currentSpeed * Time.deltaTime;
        }

        targetPosition = calculatedPosition;
        patternRunTime += Time.deltaTime;
    }

    void PerformMovementPatternCircleCounterClockwise()  //circle around goliath clockwise
    {
        if (patternRunTime > circleTime)
        {
            currentMovementPattern = -1;
            circleDistanceCovered = 0;
            return;
        }

        Vector3 rotationAxis = new Vector3(0, 0, 1);

        float positionInCircle = circleDistanceCovered / (2 * circlingDistance * Mathf.PI); //circumference = 2*pi*r
        if (positionInCircle >= 1f)
        {
            positionInCircle -= 1f;
        }
        Quaternion q = Quaternion.AngleAxis(360f * positionInCircle, rotationAxis);
        Vector2 DefaultDistance = new Vector2(circlingDistance, circlingDistance);
        Vector2 calculatedPosition = (Vector2)(q * DefaultDistance) + assumedGoliathPosition;
        if (!targetPositionInitialized) //warps avatar into place
        {
            targetPosition = calculatedPosition;
            avatarRb.position = calculatedPosition;
            targetPositionInitialized = true;
            return;
        }

        if (!(Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance || Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance)) //when avatar is too far off the path, should pause the pattern until they're back on track
        {
            circleDistanceCovered += currentSpeed * Time.deltaTime;
        }

        targetPosition = calculatedPosition;
        patternRunTime += Time.deltaTime;
    }

    void PerformMovementPatternRapidBlinks()    //teleport repeatedly, including if god takes damage
    {
        if (patternRunTime > timeBetweenBlinks || currentBlinks == 0 || damageTakenInPattern > 0f)
        {
            patternRunTime = 0;
            damageTakenInPattern = 0;
            currentBlinks += 1;
            if (currentBlinks >= totalBlinks)
            {
                currentBlinks = 0;
                currentMovementPattern = -1;
                return;
            }
            float distanceFromGoliath = Random.Range(0, blinkVariance) + blinkDistance;   //fixed distance that avatar will move away from goliath; choose a random x distance, then use the matching y distance
            float randomXMin = -distanceFromGoliath;
            if (goliathRb.position.x + randomXMin <= -(arenaSize - hideMargin)) //would spawn too close to/in the wall, adjust
            {
                randomXMin = -arenaSize + hideMargin - goliathRb.position.x;
            }

            float randomXMax = distanceFromGoliath;
            if (goliathRb.position.x + randomXMax >= (arenaSize - hideMargin))
            {
                randomXMax = arenaSize - hideMargin - goliathRb.position.x;
            }

            float xDistance = Random.Range(randomXMin, randomXMax);
            targetPosition.x = goliathRb.position.x + xDistance;
            float yDistance = distanceFromGoliath - Mathf.Abs(xDistance);

            if (goliathRb.position.y - yDistance <= -(arenaSize - hideMargin))  //if can't place below goliath, go above goliath and vice versa. Only randomly pick if both are valid
            {
                targetPosition.y = goliathRb.position.y + yDistance + hideMargin;
            }
            else if (goliathRb.position.y + yDistance >= (arenaSize - hideMargin))
            {
                targetPosition.y = goliathRb.position.y - yDistance - hideMargin;
            }
            else
            {
                int ySign = Random.Range(0, 2);
                if (ySign == 0)
                {
                    targetPosition.y = goliathRb.position.y + yDistance;
                }
                else
                {
                    targetPosition.y = goliathRb.position.y - yDistance;
                }
            }

            avatarRb.position = targetPosition;
        }
        patternRunTime += Time.deltaTime;
    }

    void PerformMovementPatternFlee()
    {
        if (patternRunTime >= hideTime)
        {
            patternRunTime = 0;
            damageTakenInPattern = 0;
            currentMovementPattern = 0;
            return;
        }
        else if (patternRunTime == 0)
        {
            float distanceFromGoliath = Random.Range(0, hideVariance) + hideDistance;   //fixed distance that avatar will move away from goliath; choose a random x distance, then use the matching y distance
            float randomXMin = -distanceFromGoliath;
            if (goliathRb.position.x + randomXMin <= -(arenaSize - hideMargin)) //would spawn too close to/in the wall, adjust
            {
                randomXMin = -arenaSize + hideMargin - goliathRb.position.x;
            }

            float randomXMax = distanceFromGoliath;
            if (goliathRb.position.x + randomXMax >= (arenaSize - hideMargin))
            {
                randomXMax = arenaSize - hideMargin - goliathRb.position.x;
            }

            float xDistance = Random.Range(randomXMin, randomXMax);
            targetPosition.x = goliathRb.position.x + xDistance;
            float yDistance = distanceFromGoliath - Mathf.Abs(xDistance);

            if (goliathRb.position.y - yDistance <= -(arenaSize - hideMargin))  //if can't place below goliath, go above goliath and vice versa. Only randomly pick if both are valid
            {
                targetPosition.y = goliathRb.position.y + yDistance + hideMargin;
            }
            else if (goliathRb.position.y + yDistance >= (arenaSize - hideMargin))
            {
                targetPosition.y = goliathRb.position.y - yDistance - hideMargin;
            }
            else
            {
                int ySign = Random.Range(0, 2);
                if (ySign == 0)
                {
                    targetPosition.y = goliathRb.position.y + yDistance;
                }
                else
                {
                    targetPosition.y = goliathRb.position.y - yDistance;
                }
            }

            //avatarRb.position = targetPosition;
        }
        patternRunTime += Time.deltaTime;
    }

    void ApplyMovement()
    {
        Vector2 targetDirection = targetPosition - avatarRb.position;
        targetDirection.Normalize();
        float speedForFrame = Time.deltaTime * currentSpeed;
        bool increaseSpeed = true;

        Vector2 movePosition = avatarRb.position + (targetDirection * speedForFrame);

        if (Mathf.Abs(movePosition.x - targetPosition.x) < Mathf.Abs(targetDirection.x * speedForFrame)) //prevent overshooting the goal
        {
            movePosition.x = targetPosition.x;
            increaseSpeed = false;
        }

        if (Mathf.Abs(movePosition.y - targetPosition.y) < Mathf.Abs(targetDirection.y * speedForFrame))
        {
            movePosition.y = targetPosition.y;
            increaseSpeed = false;
        }

        avatarRb.MovePosition(movePosition);

        if (increaseSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed -= acceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, minSpeed);
        }
        
    }

    public override bool TakeDamage(float damage, bool fromGoliath, float invincibilityDuration)
    {
        Debug.Log("checking that this is getting called");
        bool tookDamage = base.TakeDamage(damage, fromGoliath, invincibilityDuration);
        if (tookDamage)
        {
            damageTakenInPattern += damage * damageMultiplier;
        }
        Debug.Log("damage taken in pattern: " + damageTakenInPattern);
        return true;
    }
}
