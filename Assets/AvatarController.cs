using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AvatarController : Killable
{
    private Rigidbody2D avatarRb;
    private Rigidbody2D goliathRb;

    private int currentMovementPattern = 1;    //-1 means unset, 0 = circle clockwise, 1 = circle counter, 2 = short-range teleporting, 3 = hiding, 4 = running
    private int previousMovementPattern = 3;
    private float damageTakenInPattern = 0f;
    private float damageLimit = 100f;

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

    private float minBlinkRange = 8f;
    private float maxBlinkRange = 10f;
    private int totalBlinks = 5;
    private int currentBlinks = 0;
    private float timeBetweenBlinks = 5f;
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
                    PerformMovementPatternHide(); movementDone = true; break;
                case 4:
                    PerformMovementPatternFlee(); movementDone = true; break;
            }
        }
    }

    private void SelectMovementPattern()    //pick one of the 5 movement patterns, excluding the last one chosen
    {
        currentMovementPattern = Random.Range(0, 4);
        if (currentMovementPattern >= previousMovementPattern)
        {
            currentMovementPattern += 1;
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
            return;
        }

        Vector3 rotationAxis = new Vector3(0, 0, 1);
        Quaternion q = Quaternion.AngleAxis(-360f * (patternRunTime / circleTime), rotationAxis);
        Vector2 DefaultDistance = new Vector2(circlingDistance, circlingDistance);
        Vector2 calculatedPosition = (Vector2) (q * DefaultDistance) + assumedGoliathPosition;
        if (!targetPositionInitialized) //warps avatar into place
        {
            targetPosition = calculatedPosition;
            avatarRb.position = calculatedPosition;
            targetPositionInitialized = true;
            return;
        }

        if (Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance || Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance) //when avatar is too far off the path, should pause the pattern until they're back on track
        {
            //Debug.Log("avatar is off-track, pausing pattern");
        }
        else
        {
            targetPosition = calculatedPosition;
            patternRunTime += Time.deltaTime;
        }
    }

    void PerformMovementPatternCircleCounterClockwise() //circle around goliath counter-clockwise
    {
        if (patternRunTime > circleTime)
        {
            currentMovementPattern = -1;
            return;
        }

        Vector3 rotationAxis = new Vector3(0, 0, 1);
        Quaternion q = Quaternion.AngleAxis(360f * (patternRunTime / circleTime), rotationAxis);
        Vector2 DefaultDistance = new Vector2(circlingDistance, circlingDistance);
        Vector2 calculatedPosition = (Vector2)(q * DefaultDistance) + assumedGoliathPosition;
        if (!targetPositionInitialized) //warps avatar into place
        {
            targetPosition = calculatedPosition;
            avatarRb.position = calculatedPosition;
            targetPositionInitialized = true;
            return;
        }

        if (Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance || Mathf.Abs(calculatedPosition.x - avatarRb.position.x) > circleTolerance) //when avatar is too far off the path, should pause the pattern until they're back on track
        {
            //Debug.Log("avatar is off-track, pausing pattern");
        }
        else
        {
            targetPosition = calculatedPosition;
            patternRunTime += Time.deltaTime;
        }
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
            float randomX = Random.Range(minBlinkRange, maxBlinkRange);
            float randomXMultiplier = Random.Range(1, 3);   //1 = +, 2 = -
            float randomY = Random.Range(minBlinkRange, maxBlinkRange);
            float randomYMultiplier = Random.Range(1, 3);

            if (randomXMultiplier == 1)
            {
                targetPosition.x = goliathRb.position.x + randomX;
            } else
            {
                targetPosition.x = goliathRb.position.x - randomX;
            }

            if (randomYMultiplier == 1)
            {
                targetPosition.y = goliathRb.position.y + randomY;
            }
            else
            {
                targetPosition.y = goliathRb.position.y - randomY;
            }
            avatarRb.position = targetPosition;
        }
        patternRunTime += Time.deltaTime;
    }

    void PerformMovementPatternHide()
    {

    }

    void PerformMovementPatternFlee()
    {

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
