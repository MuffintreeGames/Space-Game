using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAbility : AbilityTemplate //do long stab attack, pulls to target if connects
{
    //public float stabTime = 0.1f;
    public float grappleLength = 10f;
    //public float grappleSpeed = 0.2f;

    private bool currentlyAttacking = false;

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
        if (currentlyAttacking)
        {
            parentGoliath.StopComboAttack();
            parentGoliath.EndAbility(this);
            currentlyAttacking = false;
        }
    }

    public override void UseNormalAbility()
    {
        parentGoliath.StartComboAttack();
        currentlyAttacking = true;
        parentGoliath.StartStabAttack(grappleLength, -1, 0, true);
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyAttacking = false;
        parentGoliath.StopComboAttack();
    }
}
