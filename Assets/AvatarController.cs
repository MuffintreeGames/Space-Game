using Online;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AvatarFinishAttackEvent : UnityEvent<bool>
{

}

public class AvatarController : Killable    //script to manage AI for god avatar
{
    private Rigidbody2D avatarRb;
    private Rigidbody2D goliathRb;

    private float arenaSize = 50f;

    private int currentMovementPattern = -1;    //-1 means unset, 0 = circle clockwise, 1 = circle counter, 2 = short-range teleporting, 3 = hiding, 4 = running
    private int previousMovementPattern = 10;
    private float damageTakenInPattern = 0f;
    private float damageLimit = 100f;

    private float timeBetweenAttacks = 5f;
    private float timeUntilAttack = 5f;
    private int currentAttackPattern = -1;  //-1 means unset, 0 = circle attack, 1 = aimed laser, 2 = random circles, 3 = donut
    private int previousAttackPattern = 10;
    private float explosionRange = 20f; //maximum range at which avatar will perform explosion

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

    private bool currentlyAttacking = false;    //used to indicate when avatar is mid-attack, to prevent/modify certain movement behaviours
    public static AvatarFinishAttackEvent AvatarFinishAttack;

    public GameObject Explosion;
    public GameObject Cyclone;
    public GameObject LaserAiming;
    // Start is called before the first frame update
    new void Start()
    {
        avatarRb = GetComponent<Rigidbody2D>();
        goliathRb = GameObject.Find("Goliath").GetComponent<Rigidbody2D>();
        assumedGoliathPosition = Vector2.zero;
        currentlyAttacking = false;
        AvatarFinishAttack = new AvatarFinishAttackEvent();
        AvatarFinishAttack.AddListener(StopAttacking);
        
        
        base.Start();
        //targetPosition = new Vector2(goliathRb.position.x, goliathRb.position.y + circlingDistance);
    }

    bool blockLaserSet = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!blockLaserSet && BlockableAvatarLaser.AvatarLaserBlocked != null)
        {
            BlockableAvatarLaser.AvatarLaserBlocked.AddListener(AdjustBlockedLaser);
            blockLaserSet = true;
        }

        if (PhotonNetwork.IsConnected && !RoleManager.isGoliath)
        {
            return;
        }

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

        //Debug.DrawLine(transform.position, transform.rotation.eulerAngles, Color.blue);

        if (aiming)
        {
            AimLaserSight();
        }

        if (timeUntilAttack <= 0f)
        {
            currentlyAttacking = true;
            SelectAttackPattern();
            switch (currentAttackPattern)
            {
                case 0:
                    PerformExplosionAttack();
                    break;
                case 1:
                    PerformAimedLaserAttack();
                    break;
                case 2:
                    break;
                case 3:
                    PerformCycloneAttack();
                    break;
            }
        }
        if (currentlyAttacking && gracePeriod > 0f)
        {
            gracePeriod -= Time.deltaTime;
            if (gracePeriod <= 0f)
            {
                currentlyAttacking = false;
            }
        }

        if (!currentlyAttacking)
        {
            timeUntilAttack -= Time.deltaTime;
        }
    }

    private float gracePeriod = 0f; //time after attack where avatar is still limited
    private void StopAttacking(bool placeholder)
    {
        gracePeriod = 0.5f;
    }

    private void SelectMovementPattern()    //pick one of the 4 movement patterns, excluding the last one chosen
    {
        currentMovementPattern = Random.Range(0, 3);
        if (currentMovementPattern >= previousMovementPattern)
        {
            currentMovementPattern += 1;
            if (currentMovementPattern > 3)
            {
                currentMovementPattern = 0;
            }
        }
        previousMovementPattern = currentMovementPattern;
        damageTakenInPattern = 0f;
        patternRunTime = 0f;
        targetPositionInitialized = false;
        Debug.Log("chosen movement: " + currentMovementPattern);
    }

    private void SelectAttackPattern()    //pick one of the 4 attack patterns, excluding the last one chosen. Only do circle if goliath is close, only do donut if goliath is far
    {
        currentAttackPattern = 1;
        //currentAttackPattern = Random.Range(0, 2);
        /*if (currentAttackPattern >= previousAttackPattern)
        {
            currentAttackPattern += 1;
            if (currentAttackPattern > 3)
            {
                currentAttackPattern = 0;
            }
        }*/
        if (currentAttackPattern == 0)
        {
            if (Mathf.Abs(goliathRb.position.x - avatarRb.position.x) < explosionRange && Mathf.Abs(goliathRb.position.y - avatarRb.position.y) < explosionRange)
            {
            }
            else
            {
                currentAttackPattern = 3;
            }
        }
        previousAttackPattern = currentAttackPattern;
        timeUntilAttack = timeBetweenAttacks;
        Debug.Log("chosen attack: " + currentAttackPattern);
    }

    private void PerformExplosionAttack()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity);
            else Instantiate(Explosion, transform.position, Quaternion.identity);
        }
    }

    private void PerformCycloneAttack()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(Cyclone.name, transform.position, Quaternion.identity);
            else Instantiate(Cyclone, transform.position, Quaternion.identity);
        }
    }

    private bool aiming = false;
    private Transform laserSightTransform = null;
    void PerformAimedLaserAttack()
    {
        aiming = true;
        if (PhotonNetwork.IsConnected)
        {
            Vector2 targetDirection = assumedGoliathPosition - (Vector2) transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
            GameObject instantiatedLaser;
            if (PhotonNetwork.IsConnected) instantiatedLaser = PhotonNetwork.Instantiate(LaserAiming.name, (Vector2) transform.position + (GetComponent<CircleCollider2D>().radius * transform.localScale * targetDirection.normalized * 1.1f), targetRotation);
            else instantiatedLaser = Instantiate(LaserAiming, (Vector2)transform.position + (GetComponent<CircleCollider2D>().radius * transform.localScale * targetDirection.normalized * 1.1f), Quaternion.identity);

            laserSightTransform = instantiatedLaser.GetComponent<Transform>();
        }
    }

    void AimLaserSight()
    {
        if (laserSightTransform == null)
        {
            aiming = false;
            return;
        }

        
        Vector2 targetDirection = assumedGoliathPosition - (Vector2)transform.position;
        laserSightTransform.position = (Vector2)transform.position + (GetComponent<CircleCollider2D>().radius * transform.localScale * targetDirection.normalized * 1.1f);
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        laserSightTransform.rotation = targetRotation;
    }

    private void AdjustBlockedLaser(Vector2 blockingCoords, Transform laserTransform)  //if there's a large enough obstacle touching the laser, it should get blocked. If multiple such objects exist, only the one closest to the goliath should work
    {
        //Vector2 currentGrappleTarget = (Vector2)grappledObject.transform.position + relativeGrapplePoint;
        Vector2 targetDirection = blockingCoords - (Vector2) laserTransform.position;
        float currentDistance = Mathf.Sqrt(targetDirection.x * targetDirection.x + targetDirection.y * targetDirection.y);
        //if (laserLength > currentDistance)
        //{
            //laserLength = currentDistance;
            laserTransform.localScale = new Vector2(laserTransform.localScale.x, currentDistance);
         //   skipFrame = true;
        //}
        //float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        //goliathTongue.transform.rotation = targetRotation;
        //goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, currentDistance / goliathTransform.localScale.y, goliathTongue.transform.localScale.z);
        //goliathRigid.velocity = targetDirection.normalized * grappleSpeed / slowComponent.GetSlowFactor();
    }

    void CalculateGoliathPosition()
    {
        float speedForFrame = Time.deltaTime * goliathPositionChangeSpeed;
        /*if (currentlyAttacking)
        {
            speedForFrame /= 2; //slow down avatar movement during attack
        }*/
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
            if (currentlyAttacking)
            {
                circleDistanceCovered += currentSpeed * Time.deltaTime / 2;
            }
            else
            {
                circleDistanceCovered += currentSpeed * Time.deltaTime;
            }
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
            if (currentlyAttacking)
            {
                circleDistanceCovered += currentSpeed * Time.deltaTime / 2;
            }
            else
            {
                circleDistanceCovered += currentSpeed * Time.deltaTime;
            }
        }

        targetPosition = calculatedPosition;
        patternRunTime += Time.deltaTime;
    }

    void PerformMovementPatternRapidBlinks()    //teleport repeatedly, including if god takes damage
    {
        if ((patternRunTime > timeBetweenBlinks || currentBlinks == 0 || damageTakenInPattern > 0f) && !currentlyAttacking) //don't blink while attacking
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
        if (currentlyAttacking)
        {
            speedForFrame /= 2;
        }

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

    private void OnDestroy()
    {
        //int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (TimeManager.gameOver)    //not in game anymore
        {
            return;
        }

        if (PhotonNetwork.IsConnected) GameManager.Instance.LeaveRoom();
        if (RoleManager.isGoliath)  //go to win screen if goliath, otherwise go to lose screen
        {
            TimeManager.gameOver = true;
            SceneManager.LoadScene("WinScreen");
            
        }
        else
        {
            TimeManager.gameOver = true;
            SceneManager.LoadScene("LoseScreen");
        }
    }

    [PunRPC]
    void Killed(bool fromGoliath)   //this is on Killable too, but needs to be separately implemented here to work properly
    {
        Debug.Log("calling killed via RPC for " + gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(PhotonView.Get(this));
        }
    }
}
