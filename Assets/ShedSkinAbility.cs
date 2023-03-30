using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShedSkinAbility : BuffAbility  //dash a short distance, maintaining some speed afterwards
{
    public float speedMultiplier = 2f;   //multiplier applied to goliath max speed, acceleration, and deceleration
    public float damageMultiplier = 1.2f;   //multiplier applied to damage taken by goliath

    private float currentDuration = 0f;
    private bool currentlyDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Buff);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyDashing)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= effectDuration)
            {
                CancelAbility();
            }
        }
    }

    public override void UseNormalAbility()
    {
        parentGoliath.ApplySpeedMultiplier(speedMultiplier);
        parentGoliath.ApplyDamageTakenMultiplier(damageMultiplier);
        currentDuration = 0f;
        currentlyDashing = true;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyDashing = false;
        parentGoliath.ResetSpeed();
        parentGoliath.RemoveDamageTakenMultiplier(damageMultiplier);
    }
}
