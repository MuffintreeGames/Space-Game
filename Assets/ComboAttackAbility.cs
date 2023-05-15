using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttackAbility : AbilityTemplate //do 3 basic attacks in rapid succession
{
    public float swingTime = 0.15f;

    private bool currentlyAttacking = false;
    private int attacksLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Attack);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }

    protected override void ContinueCombo()
    {
        Debug.Log("continuing combo");
        if (currentlyAttacking)
        {
            if (attacksLeft <= 0)
            {
                parentGoliath.StopComboAttack();
                parentGoliath.EndAbility(this);
                currentlyAttacking = false;
            }
            else
            {
                if (attacksLeft % 2 == 0)
                {
                    parentGoliath.StartBasicAttack(swingTime, true);
                } else
                {
                    parentGoliath.StartBasicAttack(swingTime, false);
                }
                attacksLeft -= 1;
            }
        }
    }

    public override void UseNormalAbility()
    {
        parentGoliath.StartComboAttack();
        currentlyAttacking = true;
        parentGoliath.StartBasicAttack(swingTime, false);
        attacksLeft = 2;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyAttacking = false;
        parentGoliath.StopComboAttack();
        attacksLeft = 0;
    }
}
