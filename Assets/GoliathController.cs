using Online;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GoliathLevelupEvent : UnityEvent<int>
{

}

public class GoliathFinishAttackEvent : UnityEvent
{

}

public class GoliathEquipAbilityEvent : UnityEvent<AbilityTemplate>
{

}

public class GoliathController : MonoBehaviour  //responsible for handling of player character
{
    private GameObject goliath; //the goliath that this script is attached to
    private Transform goliathTransform; //the transform of the goliath
    private GameObject goliathArm;  //the arm of the goliath, used for basic attacks. Should be the first child of the goliath object
    private GameObject goliathTongue;   //tongue of the goliath, used for stab attacks.
    private GameObject goliathTongueHitbox; //separate object used to store hitbox for tongue

    private Killable goliathHealth;

    private AttackObject ramHitboxScript; //script that controls the goliath's damage-on-touch hitbox

    private AttackObject goliathArmScript;  //script attached to the goliath arm
    private AttackObject goliathTongueScript;  //script attached to the goliath tongue
    private GrappleOnTouch goliathTongueGrappleScript;  //script attached to goliath tongue to allow grappling

    private float maxSpeed = 7.5f;  //top speed goliath can achieve; maintained separately for horizontal and vertical
    private float sizeSpeedMultiplier = 1f; //affects acceleration + max speed; increases as goliath size increases
    private float currentHorAcceleration = 0f; //acceleration value that we're currently using
    private float currentVertAcceleration = 0f; //acceleration value that we're currently using
    private float acceleration = 3f;  //rate of acceleration; maintained separately for horizontal and vertical
    private float reversingAcceleration = 8f;   //alt acceleration used when trying to go in the opposite direction
    private float deceleration = 4f;    //rate of slowing down when not moving in the direction of motion
    private bool movementLocked = false;    //used when something is going on to prevent goliath movement

    private float rotationSpeed = 840f; //speed that sprite will spin to face the chosen direction

    private float armSwingTime = 0.2f; //total time that arm should take to complete swing
    private float basicAttackCooldown = 0.5f; //required time that must elapse after a swing is finished before another swing can start
    private float currentSwingTime = 0f; //time elapsed in swing so far
    private float currentFullSwingTime = 0f;    //time that basic swing should take
    private float currentBasicAttackCooldown = 0f; //time elapsed after swing so far
    private bool canMeleeAttack = true; //determines if melee attack is legal
    private bool performingBasicAttack = false; //determines if basic attack is underway
    private bool performingComboAttack = false; //if true, keep doing basic attacks until finished
    private bool mirroredAttack = false;    //if true, attack in opposite direction
    private bool inBasicAttackCooldown = false; //determines if basic attack cooldown is ticking

    public Color chargedColor = Color.yellow;
    private int tongueDamage = 20;
    private float stabChargeTime = 0.7f;    //time to hold down basic attack button to prepare stab
    private float tongueMaxLength = 2f; //max default tongue extension distance
    private float tongueExtendTime = 0.1f;  //time to reach max extension on tongue
    private float currentTongueExtendTime = 0.1f;
    private float tongueHoldTime = 0.1f;  //time to hold out tongue after extending
    private bool performingStabAttack = false;  //determines if stab attack is underway
    private float currentStabTime = 0f; //time elapsed in stab so far
    private float currentMaxLength = 2f;
    private bool currentlyGrappling = false;
    private Vector2 relativeGrapplePoint;
    private GameObject grappledObject;
    private float grappleExtendTime = 0.2f;
    private float grappleSpeed = 10f;
    private LayerMask grappleNonAdjustLayers;  //layers where we shouldn't adjust the target of the grapple; mostly big square things
    private float maxGrappleTime = 3f;  //if grapple takes longer than this, assume goliath is stuck and cancel grapple
    private float currentGrappleTime;

    private int currentExp = 0; //current exp of the goliath
    private int neededExp = 100;    //needed exp to level up; initial value is amount needed to reach level 2

    private int level3Exp = 300;    //exp to reach level 3
    private int level4Exp = 700;    //exp to reach level 4
    private int level5Exp = 1500;   //exp to reach level 5

    private int level = 1;  //size of creature, max out at 5
    private float levelHealthBonus = 100f; //extra health gained per level

    public GoliathCameraController goliathCamera;

    private float originalMaxSpeed;
    private float originalAcceleration;
    private float originalReversingAcceleration;
    private float originalDeceleration;

    public AbilityTemplate Action1; //ability tied to the action1 button
    public AbilityTemplate Action2; //ability tied to the action2 button
    public AbilityTemplate Action3; //ability tied to the action3 button
    public AbilityTemplate Action4; //ability tied to the action4 button

    private bool abilitySelectionMode = false;   //if true, player is choosing which ability to replace if any
    private AbilityTemplate abilitySelectionOption = null;  //ability that is being considered during ability selection

    private AbilityTemplate activeAbility = null;  //if a dash-type ability is active, put here. Should be cancelled on collision with something solid

    public static GoliathLevelupEvent GoliathLevelup;
    public static GoliathFinishAttackEvent GoliathFinishAttack; //used for telling attack abilities when an attack is done
    public static GoliathEquipAbilityEvent GoliathEquipAbility; //can be used for some visual/audio effect after an ability has been equipped

    private Rigidbody2D goliathRigid;

    public LayerMask damagableLayers;

    private StatusController statusController;

    private bool abilitiesLocked = false;   //used when some effect prevents goliath from using abilities

    private float paralysisLevel = 0f;  //slows down goliath as a multiplier
    private float maxParalysis = 0.85f; //cap on paralysis
    private float paralysisDuration = 2f;   //time that paralysis lasts after being applied
    private float currentParalysisDuration = 0f;    //current time elapsed in paralysis

    private float currentSizeMultiplier = 1f;

    private SlowableObject slowComponent;

    public PhotonView goliathPhoton;

    // Start is called before the first frame update

    void Awake()
    {
        GoliathLevelup = new GoliathLevelupEvent();
        GoliathFinishAttack = new GoliathFinishAttackEvent();
        GoliathEquipAbility = new GoliathEquipAbilityEvent();
    }
    void Start()
    {
        goliath = this.gameObject;
        goliathTransform = goliath.transform;
        goliathRigid = GetComponent<Rigidbody2D>();
        goliathArm = goliathTransform.GetChild(0).gameObject;
        goliathArmScript = goliathArm.transform.GetChild(0).GetComponent<AttackObject>();
        goliathTongue = goliathTransform.GetChild(3).gameObject;
        goliathTongueHitbox = goliathTongue.transform.GetChild(0).gameObject;
        goliathTongueScript = goliathTongueHitbox.GetComponent<AttackObject>();
        goliathTongueGrappleScript = goliathTongueHitbox.GetComponent<GrappleOnTouch>();

        ramHitboxScript = goliathTransform.GetChild(4).gameObject.GetComponent<AttackObject>();

        goliathHealth = goliath.GetComponent<Killable>();

        EXPSource.GoliathGainExp.AddListener(GainExp);
        GrantAbility.GoliathGainAbility.AddListener(GainAbility);

        originalMaxSpeed = maxSpeed;
        originalAcceleration = acceleration;
        originalReversingAcceleration = reversingAcceleration;
        originalDeceleration = deceleration;

        damagableLayers = (1 << LayerMask.NameToLayer("DestructibleSize1"));
        damagableLayers |= (1 << LayerMask.NameToLayer("GoliathDestructible"));
        damagableLayers |= (1 << LayerMask.NameToLayer("AlienShip"));
        damagableLayers |= (1 << LayerMask.NameToLayer("Avatar"));

        statusController = GameObject.Find("StatusContainer").GetComponent<StatusController>();
        slowComponent = GetComponent<SlowableObject>();

        grappleNonAdjustLayers |= (1 << LayerMask.NameToLayer("BarrierLevel1"));
        grappleNonAdjustLayers |= (1 << LayerMask.NameToLayer("BarrierLevel2"));
        grappleNonAdjustLayers |= (1 << LayerMask.NameToLayer("BarrierLevel3"));

        goliathPhoton = PhotonView.Get(this);
        if (PhotonNetwork.IsConnected && !RoleManager.isGoliath)
        {
            goliathArmScript.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            goliathTongueScript.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void SetGoliathRotation()
    {
        if (!PhotonNetwork.IsConnected || RoleManager.isGoliath)
        {
            if (performingBasicAttack || performingStabAttack)  //lock rotation while attacking
            {
                return;
            }

            if (movementLocked)
            {
                return;
            }



            float horizontalDirection = Input.GetAxisRaw("Horizontal");
            float verticalDirection = Input.GetAxisRaw("Vertical");

            float targetRotation = goliathRigid.rotation;
            float currentRotation = goliathRigid.rotation;

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
            }
            else
            {    //moving vertically
                if (verticalDirection > 0f)
                {
                    targetRotation = 360f;
                }
                else if (verticalDirection < 0f)
                {
                    targetRotation = 180f;
                }
            }

            if (currentRotation < 0f)
            {
                currentRotation += 360f;
            }
            else if (currentRotation >= 360f)
            {
                currentRotation -= 360f;
            }

            float gapBetweenRotations = targetRotation - currentRotation;
            if (Mathf.Abs(gapBetweenRotations) > 180f)
            {
                if (currentRotation > 180f)
                {
                    currentRotation = -(360f - currentRotation);
                }
                else
                {
                    targetRotation = -(360f - targetRotation);
                }
            }

            if (currentRotation != targetRotation)
            {
                float smoothRotation = currentRotation;
                if (currentRotation > targetRotation)
                {
                    smoothRotation -= rotationSpeed * Time.deltaTime;
                    smoothRotation = Mathf.Max(smoothRotation, targetRotation);
                }
                else
                {
                    smoothRotation += rotationSpeed * Time.deltaTime * (1f - paralysisLevel);
                    smoothRotation = Mathf.Min(smoothRotation, targetRotation);
                }
                goliathRigid.SetRotation(smoothRotation);
            }
        }
    }

    void SetGoliathSpeed()  //set acceleration based on goliath state
    {
        if (movementLocked)
        {
            currentHorAcceleration = 0f;
            currentVertAcceleration = 0f;
            return;
        }
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");

        if (horizontalDirection != 0f)   //set horizontal accel
        {
            if (goliathRigid.velocity.x * horizontalDirection < 0f)   //accelerate faster when going the opposite direction to reach a standstill sooner
            {
                currentHorAcceleration = reversingAcceleration;
            }
            else
            {
                currentHorAcceleration = acceleration;
            }
        }


        if (verticalDirection != 0f)   //set vertical accel
        {
            if (goliathRigid.velocity.y * verticalDirection < 0f)   //accelerate faster when going the opposite direction to reach a standstill sooner
            {
                currentVertAcceleration = reversingAcceleration;
            }
            else
            {
                currentVertAcceleration = acceleration;
            }
        }
    }

    void MoveGoliath()  //apply movement based off of current speed settings
    {
        if (movementLocked)
        {
            return;
        }


        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");
        float currentMaxSpeed = maxSpeed * sizeSpeedMultiplier;

        if (horizontalDirection != 0f && verticalDirection != 0f)
        {  //reduce speed/acceleration when moving diagonally
            currentMaxSpeed = Mathf.Sqrt(2*(maxSpeed * maxSpeed));
            float accelerationMultiplier = Mathf.Sqrt((acceleration * acceleration) / 2) / acceleration;
            currentHorAcceleration *= accelerationMultiplier;
            currentVertAcceleration *= accelerationMultiplier;
        }


        Vector2 currentVelocity = goliathRigid.velocity;
        if (!(currentVelocity.x > (currentMaxSpeed / slowComponent.GetSlowFactor()) && horizontalDirection > 0) && !((currentVelocity.x < -(currentMaxSpeed / slowComponent.GetSlowFactor()) && horizontalDirection < 0)))
        {
            goliathRigid.AddForce(new Vector2(1, 0) * horizontalDirection * currentHorAcceleration * sizeSpeedMultiplier * (1f - paralysisLevel) * goliathRigid.mass / slowComponent.GetSlowFactor());
        }

        if (!(currentVelocity.y > (currentMaxSpeed / slowComponent.GetSlowFactor()) && verticalDirection > 0) && !((currentVelocity.y < -(currentMaxSpeed / slowComponent.GetSlowFactor()) && verticalDirection < 0)))
        {
            goliathRigid.AddForce(new Vector2(0, 1) * verticalDirection * currentVertAcceleration * sizeSpeedMultiplier * (1f - paralysisLevel) * goliathRigid.mass / slowComponent.GetSlowFactor());
        }

        /*Vector2 finalVelocity = goliathRigid.velocity;
        if (finalVelocity.x > maxSpeed)
        {
            finalVelocity.x = maxSpeed;
        } else if (finalVelocity.x < -maxSpeed)
        {
            finalVelocity.x = -maxSpeed;
        }

        if (finalVelocity.y > maxSpeed)
        {
            finalVelocity.y = maxSpeed;
        }
        else if (finalVelocity.y < -maxSpeed)
        {
            finalVelocity.y = -maxSpeed;
        }

        goliathRigid.velocity = finalVelocity;*/
    }

    public void StartBasicAttack(float timeToFinish, bool mirrored = false) //activate arm, disable ability to perform further attacks
    {
        Debug.Log(performingComboAttack);
        if (!canMeleeAttack && !performingComboAttack)  //skip normal cooldown if in combo attack
        {
            Debug.Log("can't attack right now");
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            goliathPhoton.RPC("StartSwingAttack", RpcTarget.All, timeToFinish, mirrored);
            return;
        }
        goliathArm.SetActive(true);
        currentSwingTime = 0f;
        currentFullSwingTime = timeToFinish;
        canMeleeAttack = false;
        mirroredAttack = mirrored;
        performingBasicAttack = true;
        ContinueBasicAttack();
    }

    public void StartComboAttack()
    {
        performingComboAttack = true;
    }

    public void StopComboAttack()
    {
        performingComboAttack = false;
    }

    void ContinueBasicAttack()  //swing arm along attack arc
    {
        if (currentSwingTime >= currentFullSwingTime)
        {
            EndBasicAttack();
            return;
        }
        float targetArmAngle;

        if (mirroredAttack)
        {
            targetArmAngle = -90f - (180f * (currentSwingTime / currentFullSwingTime));   //arm should start at -90f and end at 90f
            //targetArmAngle = 90f - (180f * (currentSwingTime / currentFullSwingTime));   //arm should start at 90f and end at -90f
            goliathArm.transform.localScale = new Vector2(goliathArm.transform.localScale.x, Mathf.Abs(goliathArm.transform.localScale.y) * -1);
        }
        else
        {
            targetArmAngle = -90f + (180f * (currentSwingTime / currentFullSwingTime));   //arm should start at -90f and end at 90f
            goliathArm.transform.localScale = new Vector2(goliathArm.transform.localScale.x, Mathf.Abs(goliathArm.transform.localScale.y));
        }
        goliathArm.transform.localEulerAngles = new Vector3(goliathArm.transform.eulerAngles.x, goliathArm.transform.eulerAngles.y, targetArmAngle);
        //Debug.Log("current arm angle: " + goliathArm.transform.eulerAngles);
        currentSwingTime += Time.deltaTime;
    }

    void EndBasicAttack()   //disable arm, start cooldown
    {
        Debug.Log("ending attack sequence");
        
        if (PhotonNetwork.IsConnected)
        {
            goliathPhoton.RPC("EndSwingAttack", RpcTarget.All);
            return;
        }
        goliathArm.transform.GetChild(0).gameObject.GetComponent<AttackObject>().ClearHitTargets();
        goliathArm.SetActive(false);
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        performingBasicAttack = false;
        GoliathFinishAttack.Invoke();
    }

    public void StartStabAttack(float maxLength, float specialTime = -1, int specialDamage = -1, bool grapple = false)
    {
        if (!canMeleeAttack && !performingComboAttack)
        {
            Debug.Log("can't attack right now");
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            goliathPhoton.RPC("StartChargedAttack", RpcTarget.All, maxLength, specialTime, specialDamage, grapple);
            return;
        }
        goliathTongue.SetActive(true);
        //goliathTongueHitbox.SetActive(true);
        currentStabTime = 0f;
        canMeleeAttack = false;
        performingStabAttack = true;
        currentMaxLength = maxLength;

        if (specialDamage >= 0) //using non-standard damage; otherwise, use default stab damage
        {
            goliathTongueScript.Damage = specialDamage;
        } else
        {
            goliathTongueScript.Damage = tongueDamage;
        }

        goliathTongueGrappleScript.enabled = grapple;
        if (grapple)
        {
            currentTongueExtendTime = grappleExtendTime;
        }
        else
        {
            currentTongueExtendTime = tongueExtendTime;
        }
        ContinueStabAttack();
    }

    void ContinueStabAttack()  //extend tongue until reach max length, then hold
    {
        currentStabTime += Time.deltaTime;

        if (currentStabTime >= currentTongueExtendTime)
        {
            float totalStabTime = currentTongueExtendTime + tongueHoldTime;
            if (currentStabTime >= totalStabTime)
            {
                EndStabAttack();
                return;
            }
        } else
        {
            float tongueExtension = currentMaxLength * (currentStabTime/currentTongueExtendTime);
            goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, tongueExtension, goliathTongue.transform.localScale.z);
            //goliathTongueHitbox.GetComponent<Rigidbody2D>().MovePosition(transform.position + transform.TransformDirection(Vector2.up * tongueExtension * transform.localScale.y)/*new Vector2(goliathTongue.transform.position.x, transform.position.y + tongueExtension)*/);
            //Debug.Log("current arm angle: " + goliathArm.transform.eulerAngles);
        }
        
    }

    void EndStabAttack()   //disable tongue, start cooldown
    {
        Debug.Log("ending stab sequence");
        if (PhotonNetwork.IsConnected)
        {
            goliathPhoton.RPC("EndChargedAttack", RpcTarget.All);
            return;
        }
        goliathTongueScript.ClearHitTargets();
        goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, 0f, goliathTongue.transform.localScale.z);
        //goliathTongueHitbox.GetComponent<Rigidbody2D>().MovePosition(transform.position);
        goliathTongue.SetActive(false);
        //goliathTongueHitbox.SetActive(false);
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        performingStabAttack = false;
        GoliathFinishAttack.Invoke();
    }

    public void PerformGrapple(GameObject grappledObject, Vector2 grapplePoint)
    {
        performingStabAttack = false;
        goliathTongueScript.Damage = 0;
        goliathTongueGrappleScript.enabled = false;
        //goliathTongueHitbox.SetActive(false);
        movementLocked = true;
        currentlyGrappling = true;
        this.grappledObject = grappledObject;
        relativeGrapplePoint = grapplePoint - (Vector2) grappledObject.transform.position;  //track grapple point relative to center of target object so that we can keep the grapple in place if the object moves
        Vector2 targetDirection = grapplePoint - (Vector2)goliathTongue.transform.position;

        if (grappleNonAdjustLayers != (grappleNonAdjustLayers | (1 << grappledObject.layer)))
        {
            if ((relativeGrapplePoint.x < 0 && targetDirection.x > 0) || (relativeGrapplePoint.x > 0 && targetDirection.x < 0))
            {
                relativeGrapplePoint.x = 0;
            }
            if ((relativeGrapplePoint.y < 0 && targetDirection.y > 0) || (relativeGrapplePoint.y > 0 && targetDirection.y < 0))
            {
                relativeGrapplePoint.y = 0;
            }
        }
        currentGrappleTime = 0f;
    }

    void ContinueGrapple()
    {
        if (!grappledObject)
        {
            Debug.Log("no grappled object");
            EndGrapple();
        }
        
        currentGrappleTime += Time.deltaTime;
        if (currentGrappleTime >= maxGrappleTime)
        {
            Debug.Log("reached max grapple duration");
            EndGrapple();
        }
        Vector2 currentGrappleTarget = (Vector2)grappledObject.transform.position + relativeGrapplePoint;
        Vector2 targetDirection = currentGrappleTarget - (Vector2)goliathTongue.transform.position;
        float currentDistance = Mathf.Sqrt(targetDirection.x * targetDirection.x + targetDirection.y * targetDirection.y);
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        goliathTongue.transform.rotation = targetRotation;
        goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, currentDistance / goliathTransform.localScale.y, goliathTongue.transform.localScale.z);
        goliathRigid.velocity = targetDirection.normalized * grappleSpeed / slowComponent.GetSlowFactor();
        //goliathRigid.AddForce(targetDirection * currentHorAcceleration * (1f - paralysisLevel) * goliathRigid.mass / slowComponent.GetSlowFactor());
    }

    void EndGrapple()
    {
        if (!currentlyGrappling)
        {
            return;
        }
        movementLocked = false;
        currentlyGrappling = false;
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        //goliathRigid.velocity = Vector2.zero;
        goliathTongueScript.Damage = tongueDamage;
        goliathTongue.transform.localRotation = Quaternion.identity;
        goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, 0f, goliathTongue.transform.localScale.z);
        goliathTongue.SetActive(false);
        if (activeAbility)
        {
            activeAbility.CancelAbility();
        }
    }

    void CheckAbilityUsage()
    {

        if (PhotonNetwork.IsConnected && !RoleManager.isGoliath)
        {
            return;
        }
        if (abilitiesLocked)
        {
            return;
        }

        if (Input.GetButtonDown("Action1") && Action1 != null)
        {
            //if (Action1.IsOffCooldown())
            //{
                Action1.UseAbility();
            //}
        }

        if (Input.GetButtonDown("Action2") && Action2 != null)
        {
            //if (Action2.IsOffCooldown())
            //{
                Action2.UseAbility();
            //}
        }

        if (Input.GetButtonDown("Action3") && Action3 != null)
        {
            //if (Action3.IsOffCooldown())
            //{
                Action3.UseAbility();
            //}
        }

        if (Input.GetButtonDown("Action4") && Action4 != null)
        {
            //if (Action4.IsOffCooldown())
            //{
                Action4.UseAbility();
            //}
        }
    }

    void CheckAbilityReplacement()  //if player presses an ability key, assign the new ability to that
    {
        if (Input.GetButtonDown("Action1"))
        {
            PhotonView.Get(this).RPC("SetAction1", RpcTarget.All, abilitySelectionOption.displayName);
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action2"))
        {
            PhotonView.Get(this).RPC("SetAction2", RpcTarget.All, abilitySelectionOption.displayName);
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action3"))
        {
            PhotonView.Get(this).RPC("SetAction3", RpcTarget.All, abilitySelectionOption.displayName);
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action4"))
        {
            PhotonView.Get(this).RPC("SetAction4", RpcTarget.All, abilitySelectionOption.displayName);
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Cancel"))
        {
            abilitySelectionOption = null;
            abilitySelectionMode = false;
        }
    }

    private float basicAttackHeldTimer = 0f;
    private bool holdingBasicAttack = false;

    //private bool midBlink = false;
    //private Color originalColor = Color.white;

    private void FixedUpdate()  //set rotation here to avoid updating rotation faster than physics engine allows
    {
        if (!TimeManager.GetTimeRunning())   //game paused
        {
            return;
        }

        SetGoliathRotation();
        if (performingBasicAttack)
        {
            ContinueBasicAttack();
        }

        if (performingStabAttack)
        {
            ContinueStabAttack();
        }

        if (currentlyGrappling)
        {
            ContinueGrapple();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !RoleManager.isGoliath) return;

        if (!TimeManager.GetTimeRunning())   //game paused
        {
            return;
        }

        if (!abilitySelectionMode)
        {
            CheckAbilityUsage();
        }
        else  //player choosing which ability to replace, so ability keys are mapped to that instead
        {
            CheckAbilityReplacement();
        }
        ManageParalysis();
        SetGoliathSpeed();

        MoveGoliath();


        //goliathCamera.updateCamera();   //now that we've moved, set new camera position

        if (holdingBasicAttack)
        {
            basicAttackHeldTimer += Time.deltaTime;
        }

        //Debug.DrawLine(transform.position, transform.rotation.eulerAngles, Color.blue);

        if (Input.GetButtonDown("Basic Attack") && !performingComboAttack)
        {
            StartBasicAttack(armSwingTime);
            holdingBasicAttack = true;
            if (basicAttackHeldTimer >= stabChargeTime)
            {

                //Blink(chargedColor);  //should do a sound effect here probably instead of visual thing
            }
        }

        if (Input.GetButtonUp("Basic Attack") && !performingComboAttack)
        {
            if (basicAttackHeldTimer >= stabChargeTime)
            {
                StartStabAttack(tongueMaxLength);
            }

            holdingBasicAttack = false;
            basicAttackHeldTimer = 0f;
        }

        if (inBasicAttackCooldown)
        {
            currentBasicAttackCooldown -= Time.deltaTime;
            if (currentBasicAttackCooldown <= 0f)
            {
                inBasicAttackCooldown = false;
                canMeleeAttack = true;
            }
        }

    }

    private int hpCost = 5;    //amount of exp to convert to 1 point of health
    void GainExp(int exp)   //get exp, check for level up
    {
        if (level > 4)  //at max level, convert exp into healing
        {
            goliathHealth.Heal(exp / hpCost);
            return;
        }
        currentExp += exp;
    }


    /*void Blink(Color blinkColor)
    {
        SpriteRenderer goliathSprite = goliath.GetComponent<SpriteRenderer>();
        if (!midBlink)
        {
            midBlink = true;
            originalColor = goliathSprite.color;
            goliathSprite.color = blinkColor;
        } else
        {
            midBlink = false;
            goliathSprite.color = originalColor;
        }
    }*/

    public void LevelUp()  //grow bigger, set up parameters for next level. Called from the HUDManager class to sync up with exp bar animation
    {
        if (level > 4)  //5 is the max level, can't level up after level 4
        {
            Debug.LogError("trying to level up after max level!");
            return;
        }

        level += 1;
        goliathHealth.IncreaseMaxHealth(levelHealthBonus);
        currentExp -= neededExp;
        Debug.Log("leveled up! Level is now " + level);
        switch (level)
        {
            case 2:
                neededExp = level3Exp;
                goliathTransform.localScale = new Vector3(2f, 2f, 1f) * currentSizeMultiplier;
                goliathArmScript.Damage = 15;
                damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize2"));
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel1"));
                tongueDamage = 40;
                sizeSpeedMultiplier = 1.25f;
                break;
            case 3:
                neededExp = level4Exp;
                goliathTransform.localScale = new Vector3(3f, 3f, 1f) * currentSizeMultiplier;
                goliathArmScript.Damage = 20;
                damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize3"));
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel2"));
                tongueDamage = 60;
                sizeSpeedMultiplier = 1.5f;
                break;
            case 4:
                neededExp = level5Exp;
                goliathTransform.localScale = new Vector3(4f, 4f, 1f) * currentSizeMultiplier;
                goliathArmScript.Damage = 25;
                damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize4"));
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel3"));
                tongueDamage = 80;
                sizeSpeedMultiplier = 1.75f;
                break;
            case 5:
                neededExp = 1;
                currentExp = -1;
                goliathTransform.localScale = new Vector3(5f, 5f, 1f) * currentSizeMultiplier;
                goliathArmScript.Damage = 30;
                tongueDamage = 100;
                sizeSpeedMultiplier = 2f;
                break;
        }
        goliathArmScript.DamagedLayers = damagableLayers;
        goliathTongueScript.DamagedLayers = damagableLayers;
        ramHitboxScript.DamagedLayers = damagableLayers;
        SetCameraZoom();
        GoliathLevelup.Invoke(level);
    }

    public void ApplySizeMultiplier(float multiplier)
    {
        currentSizeMultiplier = multiplier * currentSizeMultiplier;
        goliathTransform.localScale = goliathTransform.localScale * multiplier;
    }

    public void ResetToDefaultSize()
    {
        currentSizeMultiplier = 1f;
        switch (level)
        {
            case 2:
                goliathTransform.localScale = new Vector3(2f, 2f, 1f);
                break;
            case 3:
                goliathTransform.localScale = new Vector3(3f, 3f, 1f);
                break;
            case 4:
                goliathTransform.localScale = new Vector3(4f, 4f, 1f);
                break;
            case 5:
                goliathTransform.localScale = new Vector3(5f, 5f, 1f);
                break;
        }
    }

    public void GainAbility(AbilityTemplate newAbility)
    {
        if (Action1 == null)
        {
            Action1 = newAbility;
            newAbility.enabled = true;
            newAbility.numOfCopies += 1;
            GoliathEquipAbility.Invoke(newAbility);
        } else if (Action2 == null)
        {
            Action2 = newAbility;
            newAbility.enabled = true;
            newAbility.numOfCopies += 1;
            GoliathEquipAbility.Invoke(newAbility);
        } else if (Action3 == null)
        {
            Action3 = newAbility;
            newAbility.enabled = true;
            newAbility.numOfCopies += 1;
            GoliathEquipAbility.Invoke(newAbility);
        } else if (Action4 == null)
        {
            Action4 = newAbility;
            newAbility.enabled = true;
            newAbility.numOfCopies += 1;
            GoliathEquipAbility.Invoke(newAbility);
        } else
        {
            abilitySelectionMode = true;
            abilitySelectionOption = newAbility;
        }
    }


        public void SetCameraZoom()
        {
            float sizeZoom = 12f; //zoom level is determined by goliath size
            switch (level)
            {
                case 1:
                    sizeZoom = 12f;
                    break;
                case 2:
                    sizeZoom = 14f;
                    break;
                case 3:
                    sizeZoom = 16f;
                    break;
                case 4:
                    sizeZoom = 18f;
                    break;
                case 5:
                    sizeZoom = 20f;
                    break;
            }
            goliathCamera.SetZoom(sizeZoom);
        }

        public void LockMovement()
        {
            movementLocked = true;
        }

        public void UnlockMovement()
        {
            movementLocked = false;
        }

        public void SetSpeedExternally(float xSpeed, float ySpeed)    //use when something other than the goliath is setting the goliath's speed
        {
        Debug.Log("setting speed with " + xSpeed + ", " +  ySpeed);
        goliathRigid.velocity = new Vector2(xSpeed, ySpeed);
        goliathRigid.AddForce(new Vector2(1, 0)  * xSpeed * goliathRigid.mass);
        goliathRigid.AddForce(new Vector2(0, 1) * ySpeed * goliathRigid.mass);
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

        public float GetHealth()
    {
        return goliathHealth.GetHealth();
    }

    public float GetMaxHealth()
    {
        return goliathHealth.MaxHealth;
    }

    public bool InAbilitySelection()
    {
        return abilitySelectionMode;
    }

    public AbilityTemplate GetSelectionOption()
    {
        return abilitySelectionOption;
    }

    public void ApplySpeedMultiplier(float speedMultiplier)
    {
        maxSpeed = maxSpeed * speedMultiplier;
        acceleration = acceleration * speedMultiplier;
        reversingAcceleration = reversingAcceleration * speedMultiplier;
        deceleration = deceleration * speedMultiplier;
    }

    public void ResetSpeed()
    {
        maxSpeed = originalMaxSpeed;
        acceleration = originalAcceleration;
        reversingAcceleration = originalReversingAcceleration;
        deceleration = originalDeceleration;
    }

    public void ApplyDamageTakenMultiplier(float damageMultiplier)
    {
        goliathHealth.ApplyDamageMultiplier(damageMultiplier);
    }

    public void RemoveDamageTakenMultiplier(float damageMultiplier)
    {
        goliathHealth.RemoveDamageMultiplier(damageMultiplier);
    }

    public void ApplyDamageDoneMultiplier(float damageMultiplier)
    {
        goliathArmScript.ApplyDamageMultiplier(damageMultiplier);
        goliathTongueScript.ApplyDamageMultiplier(damageMultiplier);
    }

    public void RemoveDamageDoneMultiplier(float damageMultiplier)
    {
        goliathArmScript.RemoveDamageMultiplier(damageMultiplier);
        goliathTongueScript.RemoveDamageMultiplier(damageMultiplier);
    }

    public void LockAbilities()
    {
        abilitiesLocked = true;
    }

    public void UnlockAbilities()
    {
        abilitiesLocked = false;
    }

    public void ActivateRamHitbox(int damage)
    {
        ramHitboxScript.enabled = true;
        ramHitboxScript.Damage = damage;
    }

    public void DisableRamHitbox()
    {
        ramHitboxScript.enabled = false;
        ramHitboxScript.Damage = 0;
    }

    void OnCollisionStay2D(Collision2D col)
        {
        StartCoroutine(HandleCollision(col));
            
        }

    public bool StartAbility(AbilityTemplate ability)   //check if we can do an ability, then register it as being active if it's an ongoing type
    {
        if (activeAbility != null)
        {
            Debug.Log("busy with " + activeAbility);
            return false;
        }

        if (performingComboAttack)
        {
            Debug.Log("doing a persistent attack!");
            return false;
        }

        AbilityTemplate.AbilityCategory abilityType = ability.GetAbilityType();
        if (abilityType == AbilityTemplate.AbilityCategory.Dash || abilityType == AbilityTemplate.AbilityCategory.Attack)
        {
            activeAbility = ability;
        } else if (abilityType == AbilityTemplate.AbilityCategory.Buff)
        {
            BuffAbility buff = ability as BuffAbility;
            statusController.AddStatus(buff.icon, buff.effectDuration);
        }
        return true;
    }

    public void EndAbility(AbilityTemplate ability)
    {
        if (activeAbility == ability)
        {
            activeAbility = null;
        }
    }

    public void StopDashAbility()
    {
        if (activeAbility == null)
        {
            return;
        }

        if (activeAbility.GetAbilityType() == AbilityTemplate.AbilityCategory.Dash)
        {
            activeAbility.CancelAbility();
            activeAbility = null;
        }
    }

    public void ResetState()    //use to cancel out of everything the goliath is doing
    {
        StopDashAbility();
        activeAbility = null;
        EndBasicAttack();
        EndStabAttack();
        EndGrapple();
        StopComboAttack();
    }
    IEnumerator HandleCollision(Collision2D col)
    {
        //yield return 0; //wait 1 frame, then check if gameObject still exists. If it doesn't, then it was destroyed on impact and we shouldn't care
        try
        {
            if (col.gameObject == null)
            {
                yield break;
            }
        } catch (Exception e)
        {
            yield break;
        }

        if (col.gameObject == grappledObject)
        {
            EndGrapple();
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize1") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize2") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize3") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize4") || col.gameObject.layer == LayerMask.NameToLayer("Solid") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel1") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel2") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel3") || col.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {
            Debug.Log("handling collision with something solid");
            StopDashAbility();
        }
    }

    public void ApplyParalysis(float paralysisAmount)
    {
        paralysisLevel += paralysisAmount;
        if (paralysisLevel > maxParalysis)
        {
            paralysisLevel = maxParalysis;
        }
        currentParalysisDuration = 0f;
    }

    public float GetSizeSpeedMultiplier() { return sizeSpeedMultiplier; }

    private void ManageParalysis()
    {
        if (paralysisLevel == 0f)
        {
            return;
        }

        currentParalysisDuration += Time.deltaTime;
        if (currentParalysisDuration > paralysisDuration)
        {
            paralysisLevel = 0f;
            currentParalysisDuration = 0f;
        }
    }

    private void OnDestroy()
    {
        //int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (TimeManager.gameOver)    //not in game anymore. Update this if any extra scenes get added to the main game
        {
            return;
        }

        if (PhotonNetwork.IsConnected) GameManager.Instance.LeaveRoom();
        if (RoleManager.isGoliath)  //go to lose screen if goliath, otherwise go to win screen
        {
            TimeManager.gameOver = true;
            SceneManager.LoadScene("LoseScreen");
        } else
        {
            TimeManager.gameOver = true;
            SceneManager.LoadScene("WinScreen");
        }
    }

    [PunRPC]
    void StartSwingAttack(float timeToFinish, bool mirrored)
    {
        goliathArm.SetActive(true);
        currentSwingTime = 0f;
        currentFullSwingTime = timeToFinish;
        canMeleeAttack = false;
        mirroredAttack = mirrored;
        performingBasicAttack = true;
        ContinueBasicAttack();
    }

    [PunRPC]
    void EndSwingAttack()
    {
        if (goliathArm.activeSelf)
        {
            goliathArm.transform.GetChild(0).gameObject.GetComponent<AttackObject>().ClearHitTargets();
            goliathArm.SetActive(false);
            currentBasicAttackCooldown = basicAttackCooldown;
            inBasicAttackCooldown = true;
            performingBasicAttack = false;
            GoliathFinishAttack.Invoke();
        }
    }

    [PunRPC]
    void StartChargedAttack(float maxLength, float specialTime, int specialDamage, bool grapple)
    {
        goliathTongue.SetActive(true);
        //goliathTongueHitbox.SetActive(true);
        currentStabTime = 0f;
        canMeleeAttack = false;
        performingStabAttack = true;
        currentMaxLength = maxLength;

        if (specialDamage >= 0) //using non-standard damage; otherwise, use default stab damage
        {
            goliathTongueScript.Damage = specialDamage;
        }
        else
        {
            goliathTongueScript.Damage = tongueDamage;
        }

        goliathTongueGrappleScript.enabled = grapple;
        if (grapple)
        {
            currentTongueExtendTime = grappleExtendTime;
        }
        else
        {
            currentTongueExtendTime = tongueExtendTime;
        }
        ContinueStabAttack();
    }

    [PunRPC]
    void EndChargedAttack()
    {
        goliathTongueScript.ClearHitTargets();
        goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, 0f, goliathTongue.transform.localScale.z);
        //goliathTongueHitbox.GetComponent<Rigidbody2D>().MovePosition(transform.position);
        goliathTongue.SetActive(false);
        //goliathTongueHitbox.SetActive(false);
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        performingStabAttack = false;
        GoliathFinishAttack.Invoke();
    }
}
