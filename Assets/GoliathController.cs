using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GoliathLevelupEvent : UnityEvent<int>
{

}

public class GoliathFinishAttackEvent : UnityEvent
{

}

public class GoliathEquipAbilityEvent : UnityEvent<AbilityTemplate>
{

}

public class GoliathController : MonoBehaviour
{
    private GameObject goliath; //the goliath that this script is attached to
    private Transform goliathTransform; //the transform of the goliath
    private GameObject goliathArm;  //the arm of the goliath, used for basic attacks. Should be the first child of the goliath object
    private GameObject goliathTongue;   //tongue of the goliath, used for stab attacks.

    private Killable goliathHealth;

    private AttackObject ramHitboxScript; //script that controls the goliath's damage-on-touch hitbox

    private AttackObject goliathArmScript;  //script attached to the goliath arm
    private AttackObject goliathTongueScript;  //script attached to the goliath tongue

    private float maxSpeed = 7.5f;  //top speed goliath can achieve; maintained separately for horizontal and vertical
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
    private float stabChargeTime = 0.7f;    //time to hold down basic attack button to prepare stab
    private float tongueMaxLength = 2f; //max default tongue extension distance
    private float tongueExtendTime = 0.1f;  //time to reach max extension on tongue
    private float tongueHoldTime = 0.1f;  //time to hold out tongue after extending
    private bool performingStabAttack = false;  //determines if stab attack is underway
    private float currentStabTime = 0f; //time elapsed in stab so far
    private float currentMaxLength = 2f;

    private int currentExp = 0; //current exp of the goliath
    private int neededExp = 100;    //needed exp to level up; initial value is amount needed to reach level 2

    private int level3Exp = 300;    //exp to reach level 3
    private int level4Exp = 700;    //exp to reach level 4
    private int level5Exp = 1500;   //exp to reach level 5

    private int level = 1;  //size of creature, max out at 5

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

    // Start is called before the first frame update

    void Start()
    {
        goliath = this.gameObject;
        goliathTransform = goliath.transform;
        goliathRigid = GetComponent<Rigidbody2D>();
        goliathArm = goliathTransform.GetChild(0).gameObject;
        goliathArmScript = goliathArm.transform.GetChild(0).GetComponent<AttackObject>();
        goliathTongue = goliathTransform.GetChild(3).gameObject;
        goliathTongueScript = goliathTongue.transform.GetChild(0).GetComponent<AttackObject>();

        ramHitboxScript = goliathTransform.GetChild(4).gameObject.GetComponent<AttackObject>();

        goliathHealth = goliath.GetComponent<Killable>();

        EXPSource.GoliathGainExp.AddListener(GainExp);
        GrantAbility.GoliathGainAbility.AddListener(GainAbility);

        GoliathLevelup = new GoliathLevelupEvent();
        GoliathFinishAttack = new GoliathFinishAttackEvent();
        GoliathEquipAbility = new GoliathEquipAbilityEvent();

        originalMaxSpeed = maxSpeed;
        originalAcceleration = acceleration;
        originalReversingAcceleration = reversingAcceleration;
        originalDeceleration = deceleration;

        damagableLayers = (1 << LayerMask.NameToLayer("DestructibleSize1"));
        damagableLayers |= (1 << LayerMask.NameToLayer("GoliathDestructible"));
    }

        void SetGoliathRotation()
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
            } else {    //moving vertically
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
        } else if (currentRotation >= 360f)
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
            } else
            {
                smoothRotation += rotationSpeed * Time.deltaTime;
                smoothRotation = Mathf.Min(smoothRotation, targetRotation);
            }
            goliathRigid.SetRotation(smoothRotation);
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


        if (verticalDirection != 0f)   //set horizontal accel
        {
            if (goliathRigid.velocity.y*verticalDirection < 0f)   //accelerate faster when going the opposite direction to reach a standstill sooner
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


        goliathRigid.AddForce(new Vector2(1, 0) * horizontalDirection * currentHorAcceleration);
        goliathRigid.AddForce(new Vector2(0, 1) * verticalDirection * currentVertAcceleration);

        Vector2 finalVelocity = goliathRigid.velocity;
        if (finalVelocity.x > (maxSpeed))
        {
            finalVelocity.x = (maxSpeed);
        } else if (finalVelocity.x < -(maxSpeed))
        {
            finalVelocity.x = -(maxSpeed);
        }

        if (finalVelocity.y > (maxSpeed))
        {
            finalVelocity.y = (maxSpeed);
        }
        else if (finalVelocity.y < -(maxSpeed))
        {
            finalVelocity.y = -(maxSpeed);
        }

        goliathRigid.velocity = finalVelocity;
    }

        public void StartBasicAttack(float timeToFinish, bool mirrored = false) //activate arm, disable ability to perform further attacks
        {
            if (!canMeleeAttack && !performingComboAttack)  //skip normal cooldown if in combo attack
            {
                Debug.Log("can't attack right now");
                return;
            }

            goliathArm.SetActive(true);
            currentSwingTime = 0f;
        currentFullSwingTime = timeToFinish;
            canMeleeAttack = false;
        mirroredAttack = mirrored;
            performingBasicAttack = true;
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
            targetArmAngle = 90f - (180f * (currentSwingTime / currentFullSwingTime));   //arm should start at 90f and end at -90f
        } else
        {
            targetArmAngle = -90f + (180f * (currentSwingTime / currentFullSwingTime));   //arm should start at -90f and end at 90f
        }
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
        GoliathFinishAttack.Invoke();
        }

    public void StartStabAttack(float maxLength)
    {
        if (!canMeleeAttack && !performingComboAttack)
        {
            Debug.Log("can't attack right now");
            return;
        }

        goliathTongue.SetActive(true);
        currentStabTime = 0f;
        canMeleeAttack = false;
        performingStabAttack = true;
        currentMaxLength = maxLength;
    }

    void ContinueStabAttack()  //extend tongue until reach max length, then hold
    {
        currentStabTime += Time.deltaTime;

        if (currentStabTime >= tongueExtendTime)
        {
            float totalStabTime = tongueExtendTime + tongueHoldTime;
            Debug.Log("current: " + currentStabTime + ", max: " + totalStabTime);
            if (currentStabTime >= totalStabTime)
            {
                EndStabAttack();
                return;
            }
        } else
        {
            float tongueExtension = currentMaxLength * (currentStabTime/tongueExtendTime);
            goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, tongueExtension, goliathTongue.transform.localScale.z);
            //Debug.Log("current arm angle: " + goliathArm.transform.eulerAngles);
        }
        
    }

    void EndStabAttack()   //disable tongue, start cooldown
    {
        Debug.Log("ending stab sequence");
        goliathTongue.transform.GetChild(0).gameObject.GetComponent<AttackObject>().ClearHitTargets();
        goliathTongue.SetActive(false);
        currentBasicAttackCooldown = basicAttackCooldown;
        inBasicAttackCooldown = true;
        performingStabAttack = false;
        GoliathFinishAttack.Invoke();
    }

    void CheckAbilityUsage()
        {
            if (Input.GetButtonDown("Action1") && Action1 != null)
            {
                if (Action1.IsOffCooldown())
                {
                    Action1.UseAbility();
                }
            }

            if (Input.GetButtonDown("Action2") && Action2 != null)
            {
                if (Action2.IsOffCooldown())
                {
                    Action2.UseAbility();
                }
            }

            if (Input.GetButtonDown("Action3") && Action3 != null)
            {
                if (Action3.IsOffCooldown())
                {
                    Action3.UseAbility();
                }
            }

            if (Input.GetButtonDown("Action4") && Action4 != null)
            {
                if (Action4.IsOffCooldown())
                {
                    Action4.UseAbility();
                }
            }
        }

    void CheckAbilityReplacement()  //if player presses an ability key, assign the new ability to that
    {
        if (Input.GetButtonDown("Action1"))
        {
            Action1.numOfCopies -= 1;
            Action1.DisableAbility();
            Action1 = abilitySelectionOption;
            abilitySelectionOption.numOfCopies += 1;
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action2"))
        {
            Action2.numOfCopies -= 1;
            Action2.DisableAbility();
            Action2 = abilitySelectionOption;
            abilitySelectionOption.numOfCopies += 1;
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action3"))
        {
            Action3.numOfCopies -= 1;
            Action3.DisableAbility();
            Action3 = abilitySelectionOption;
            abilitySelectionOption.numOfCopies += 1;
            abilitySelectionMode = false;
        } else if (Input.GetButtonDown("Action4"))
        {
            Action4.numOfCopies -= 1;
            Action4.DisableAbility();
            Action4 = abilitySelectionOption;
            abilitySelectionOption.numOfCopies += 1;
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
        SetGoliathRotation();
    }
    // Update is called once per frame
    void Update()
    {
        if (!abilitySelectionMode)
        {
            CheckAbilityUsage();
        } else  //player choosing which ability to replace, so ability keys are mapped to that instead
        {
            CheckAbilityReplacement();
        }
            
            SetGoliathSpeed();
            MoveGoliath();
            goliathCamera.updateCamera();   //now that we've moved, set new camera position

        if (holdingBasicAttack)
        {
            basicAttackHeldTimer += Time.deltaTime;
        }

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

            if (performingBasicAttack)
            {
                ContinueBasicAttack();
            }

            if (performingStabAttack)
        {
            ContinueStabAttack();
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

        void GainExp(int exp)   //get exp, check for level up
        {
            if (level > 4)  //no need for exp at max level
            {
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
            currentExp -= neededExp;
            Debug.Log("leveled up! Level is now " + level);
            switch (level)
            {
                case 2:
                    neededExp = level3Exp;
                    goliathTransform.localScale = new Vector3(2f, 2f, 1f);
                    goliathArmScript.Damage = 20;
                    damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize2"));
                    goliathTongueScript.Damage = 40;
                break;
                case 3:
                    neededExp = level4Exp;
                    goliathTransform.localScale = new Vector3(3f, 3f, 1f);
                    goliathArmScript.Damage = 30;
                damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize3"));
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel1"));
                    goliathTongueScript.Damage = 60;
                break;
                case 4:
                    neededExp = level5Exp;
                    goliathTransform.localScale = new Vector3(4f, 4f, 1f);
                    goliathArmScript.Damage = 40;
                damagableLayers |= (1 << LayerMask.NameToLayer("DestructibleSize4"));
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel2"));
                    goliathTongueScript.Damage = 80;
                break;
                case 5:
                    neededExp = 1;
                    currentExp = 0;
                    goliathTransform.localScale = new Vector3(5f, 5f, 1f);
                    goliathArmScript.Damage = 50;
                damagableLayers |= (1 << LayerMask.NameToLayer("BarrierLevel3"));
                    goliathTongueScript.Damage = 100;
                break;
            }
        goliathArmScript.DamagedLayers = damagableLayers;
        goliathTongueScript.DamagedLayers = damagableLayers;
        ramHitboxScript.DamagedLayers = damagableLayers;
            SetCameraZoom();
        GoliathLevelup.Invoke(level);
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
            float sizeZoom = 8f; //zoom level is determined by goliath size
            switch (level)
            {
                case 1:
                    sizeZoom = 8f;
                    break;
                case 2:
                    sizeZoom = 10f;
                    break;
                case 3:
                    sizeZoom = 12f;
                    break;
                case 4:
                    sizeZoom = 14f;
                    break;
                case 5:
                    sizeZoom = 16f;
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
        goliathRigid.AddForce(new Vector2(1, 0)  * xSpeed);
        goliathRigid.AddForce(new Vector2(0, 1) * ySpeed);
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

    public void ApplyDamageMultiplier(float damageMultiplier)
    {
        goliathHealth.ApplyDamageMultiplier(damageMultiplier);
    }

    public void RemoveDamageMultiplier(float damageMultiplier)
    {
        goliathHealth.RemoveDamageMultiplier(damageMultiplier);
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

    void OnCollisionEnter2D(Collision2D col)
        {
        StartCoroutine(HandleCollision(col));
            
        }

    public bool StartAbility(AbilityTemplate ability)   //check if we can do an ability, then register it as being active if it's an ongoing type
    {
        if (activeAbility != null)
        {
            return false;
        }
        AbilityTemplate.AbilityCategory abilityType = ability.GetAbilityType();
        if (abilityType == AbilityTemplate.AbilityCategory.Dash || abilityType == AbilityTemplate.AbilityCategory.Attack)
        {
            activeAbility = ability;
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

        if (col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize1") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize2") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize3") || col.gameObject.layer == LayerMask.NameToLayer("DestructibleSize4") || col.gameObject.layer == LayerMask.NameToLayer("Solid") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel1") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel2") || col.gameObject.layer == LayerMask.NameToLayer("BarrierLevel3") || col.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {
            Debug.Log("handling collision with something solid");
            StopDashAbility();
        }
    }
    }
