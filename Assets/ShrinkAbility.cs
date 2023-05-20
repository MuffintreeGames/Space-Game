using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAbility : BuffAbility  //shrinks goliath hitbox, but reduces damage done with basic attacks
{
    public float damageMultiplier = 0.6f;   //multiplier applied to goliath damage
    public float sizeMultiplier = 0.2f;

    private float currentDuration = 0f;
    private bool currentlyShrunk = false;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Buff);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyShrunk)
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
        parentGoliath.ApplyDamageDoneMultiplier(damageMultiplier);
        parentGoliath.ApplySizeMultiplier(sizeMultiplier);
        //parentGoliath.LockAbilities();
        currentDuration = 0f;
        currentlyShrunk = true;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyShrunk = false;
        parentGoliath.RemoveDamageDoneMultiplier(damageMultiplier);
        parentGoliath.ResetToDefaultSize();
    }
}
