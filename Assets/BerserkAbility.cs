using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkAbility : BuffAbility  //do increased damage with normal attacks for a time in exchange for preventing other ability use
{
    public float damageMultiplier = 2f;   //multiplier applied to goliath damage

    private float currentDuration = 0f;
    private bool currentlyBerserk = false;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Buff);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyBerserk)
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
        parentGoliath.LockAbilities();
        currentDuration = 0f;
        currentlyBerserk = true;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyBerserk = false;
        parentGoliath.RemoveDamageDoneMultiplier(damageMultiplier);
        parentGoliath.UnlockAbilities();
    }
}
