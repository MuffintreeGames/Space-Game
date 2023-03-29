using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboStabAbility : AbilityTemplate //do 3 stab attacks in rapid succession, each one getting longer
{
    //public float stabTime = 0.1f;
    public float minStabLength = 2f;
    public float maxStabLength = 4f;

    private bool currentlyAttacking = false;
    private int totalAttacks = 3;
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
                attacksLeft -= 1;
                float attackLength = minStabLength + ((maxStabLength - minStabLength) / totalAttacks * (totalAttacks - attacksLeft));
                Debug.Log(attackLength);
                parentGoliath.StartStabAttack(attackLength);
            }
        }
    }

    public override void UseNormalAbility()
    {
        parentGoliath.StartComboAttack();
        currentlyAttacking = true;
        parentGoliath.StartStabAttack(minStabLength);
        attacksLeft = totalAttacks - 1;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyAttacking = false;
        parentGoliath.StopComboAttack();
        attacksLeft = 0;
    }
}
