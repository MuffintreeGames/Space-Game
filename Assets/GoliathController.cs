using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathController : MonoBehaviour
{
    private GameObject goliath; //the goliath that this script is attached to
    private Transform goliathTransform; //the transform of the goliath
    private GameObject goliathArm;  //the arm of the goliath, used for basic attacks. Should be the first child of the goliath object

    private float maxSpeed = 7.5f;  //top speed goliath can achieve; maintained separately for horizontal and vertical
    private float acceleration = 1.5f;  //rate of acceleration; maintained separately for horizontal and vertical
    private float reversingAcceleration = 4f;   //alt acceleration used when trying to go in the opposite direction
    private float deceleration = 2f;    //rate of slowing down when not moving in the direction of motion
    private float currentHorSpeed = 0f; //current horizontal speed
    private float currentVertSpeed = 0f;    //current vertical speed

    private float rotationSpeed = 0.2f; //speed that sprite will spin to face the chosen direction

    private float reboundForce = 2f;    //force applied when bumping into something that doesn't reverse the goliath's direction
    private float oppositeReboundForce = 1f;    //force applied when bumping into something that reverses the goliath's direction

    private float armSwingTime = 0.2f; //total time that arm should take to complete swing
    private float basicAttackCooldown = 0.5f; //required time that must elapse after a swing is finished before another swing can start
    private float currentSwingTime = 0f; //time elapsed in swing so far
    private float currentBasicAttackCooldown = 0f; //time elapsed after swing so far
    private bool canBasicAttack = true; //determines if basic attack is legal
    private bool performingBasicAttack = false; //determines if basic attack is underway
    private bool inBasicAttackCooldown = false; //determines if basic attack cooldown is ticking

    private int smallPlanetExp = 5;    //exp reward for eating small planet

    private int currentExp = 0; //current exp of the goliath
    private int neededExp = 100;    //needed exp to level up; initial value is amount needed to reach level 2

    private int level3Exp = 300;    //exp to reach level 3
    private int level4Exp = 700;    //exp to reach level 4
    private int level5Exp = 1500;   //exp to reach level 5

    private int level = 1;  //size of creature, max out at 5

    public GoliathCameraController goliathCamera;

    // Start is called before the first frame update
    void Start()
    {
        goliath = this.gameObject;
        goliathTransform = goliath.transform;
        goliathArm = goliathTransform.GetChild(0).gameObject;

        Planet.GoliathSmallPlanetKilled.AddListener(EatSmallPlanet);
    }

    void SetGoliathRotation()
    {
        float targetRotation = goliathTransform.eulerAngles.z;
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");

        if (horizontalDirection == 0f && verticalDirection == 0f)   //not pressing anything, leave rotation alone
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
        //Debug.Log("current angle: " + goliathTransform.eulerAngles);    //saw issue where rotation was acting funky when rebounding, left this here in case it can help with debugging
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

    void StartBasicAttack() //activate arm, disable ability to perform further attacks
    {
        if (!canBasicAttack)
        {
            Debug.Log("can't attack right now");
            return;
        }
        Debug.Log("starting basic attack sequence");

        goliathArm.SetActive(true);
        currentSwingTime = 0f;
        canBasicAttack = false;
        performingBasicAttack = true;
    }

    void ContinueBasicAttack()  //swing arm along attack arc
    {
        if (currentSwingTime >= armSwingTime)
        {
            EndBasicAttack();
            return;
        }
        float targetArmAngle = -90f + (180f * (currentSwingTime / armSwingTime));   //arm should start at -90f and end at 90f
        goliathArm.transform.localEulerAngles = new Vector3(goliathArm.transform.eulerAngles.x, goliathArm.transform.eulerAngles.y, targetArmAngle);
        //Debug.Log("current arm angle: " + goliathArm.transform.eulerAngles);
        currentSwingTime += Time.deltaTime;
    }

    void EndBasicAttack()   //disable arm, start cooldown
    {
        Debug.Log("ending attack sequence");
        goliathArm.transform.GetChild(0).gameObject.GetComponent<AttackObject>().ClearHitTargets();
        goliathArm.SetActive(false);
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        performingBasicAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetGoliathSpeed();
        SetGoliathRotation();
        MoveGoliath();
        goliathCamera.updateCamera();   //now that we've moved, set new camera position

        if (Input.GetButtonDown("Basic Attack")) {
            StartBasicAttack();
        } 
        if (performingBasicAttack)
        {
            ContinueBasicAttack();
        }
        if (inBasicAttackCooldown)
        {
            currentBasicAttackCooldown -= Time.deltaTime;
            if (currentBasicAttackCooldown <= 0f)
            {
                inBasicAttackCooldown = false;
                canBasicAttack = true;
                Debug.Log("cooldown finished");
            }
        }
        
    }

    void EatSmallPlanet()
    {
        Debug.Log("Killed a small planet!");
        GainExp(smallPlanetExp);
    }

    void GainExp(int exp)   //get exp, check for level up
    {
        if (level > 4)  //no need for exp at max level
        {
            return;
        }
        currentExp += exp;
        /*if (currentExp >= neededExp)
        {
            LevelUp();
        }*/
    }

    public void LevelUp()  //grow bigger, set up parameters for next level. Called from the HUDManager class to sync up with exp bar animation
    {
        if (level > 4)  //5 is the max level, can't level up after level 4
        {
            Debug.LogError("trying to level up after max level!");
            return;
        }

        level += 1;
        currentExp -= neededExp;
        Debug.Log("leveled up! Level is now " + level);
        switch (level)
        {
            case 2:
                neededExp = level3Exp;
                goliathTransform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case 3:
                neededExp = level4Exp;
                goliathTransform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case 4:
                neededExp = level5Exp;
                goliathTransform.localScale = new Vector3(4f, 4f, 4f);
                break;
            case 5:
                neededExp = 1;
                currentExp = 0;
                goliathTransform.localScale = new Vector3(5f, 5f, 5f);
                break;
        }
    }

    public int GetExp()
    {
        return currentExp;
    }

    public int GetNeededExp()
    {
        return neededExp;
    }

    public int GetLevel()
    {
        return level;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize1") || col.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (currentHorSpeed * contact.normal.x < 0)
            {
                currentHorSpeed = contact.normal.x * oppositeReboundForce;
            } else
            {
                currentHorSpeed += contact.normal.x * reboundForce;
            }

            if (currentVertSpeed * contact.normal.y < 0)
            {
                currentVertSpeed = contact.normal.y * oppositeReboundForce;
            }
            else
            {
                currentVertSpeed += contact.normal.y * reboundForce;
            }
        }
    }
}
