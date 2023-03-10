using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityTemplate : MonoBehaviour
{
    public float cooldown;  //time between ability uses

    protected GoliathController parentGoliath;
    protected float tickingCooldown;
    protected bool offCooldown = true;

    protected void InitializeAbility()
    {
        parentGoliath = GetComponent<GoliathController>();
    }

    // Update is called once per frame
    protected void ManageCooldown()
    {
        if (!offCooldown) {
            tickingCooldown -= Time.deltaTime;
            if (tickingCooldown <= 0f)
            {
                offCooldown = true;
            }
        }
    }

    protected void StartCooldown()
    {
        if (!IsOffCooldown())
        {
            return;
        }
        offCooldown = false;
        tickingCooldown = cooldown;
    }

    public virtual void UseAbility()
    {
        
    }

    public virtual void CancelAbility()
    {

    }

    public bool IsOffCooldown()
    {
        return offCooldown;
    }
}
