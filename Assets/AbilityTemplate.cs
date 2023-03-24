using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbilityTemplate : MonoBehaviour
{

    public enum AbilityCategory { Attack, Dash, Projectile, Buff};

    public float cooldown;  //time between ability uses
    public Sprite icon;  //symbol for the ability

    protected GoliathController parentGoliath;
    protected float tickingCooldown;
    protected bool offCooldown = true;

    protected bool listenersAttached = false;

    protected AbilityCategory abilityType;    //should be chosen by the class that inherits this

    protected void InitializeAbility(AbilityCategory abilityType)
    {
        Debug.Log(this);
        parentGoliath = GetComponent<GoliathController>();
        UpgradeSelf(1);
        enabled = false;
        this.abilityType = abilityType;
    }

    public AbilityCategory GetAbilityType()
    {
        return abilityType;
    }

    // Update is called once per frame
    protected void ManageCooldown()
    {
        if (!listenersAttached)
        {
            if (parentGoliath)
            {
                GoliathController.GoliathLevelup.AddListener(UpgradeSelf);
                listenersAttached = true;
            }
        }

        if (!offCooldown) {
            tickingCooldown -= Time.deltaTime;
            if (tickingCooldown <= 0f)
            {
                offCooldown = true;
            }
        }
    }

    protected bool PrepareToUseAbility()    //check if ability can be used, start cooldown
    {
        if (!enabled) {   //if for whatever reason the ability has been used without being activated properly, just activate it
            enabled = true;
        }

        if (!IsOffCooldown())
        {
            Debug.Log("ability not off cooldown!");
            return false;
        }

        if (!parentGoliath.StartAbility(this))
        {
            Debug.Log("goliath doing something else!");
            return false;
        }

        offCooldown = false;
        tickingCooldown = cooldown;
        return true;
    }

    protected void PrepareToEndAbility()    //tell goliath to stop ability
    {
        parentGoliath.EndAbility(this);
    }

    public virtual void UseAbility()
    {
        
    }

    public virtual void CancelAbility()
    {

    }

    public virtual void UpgradeSelf(int goliathLevel)
    {

    }

    public bool IsOffCooldown()
    {
        return offCooldown;
    }

    public float GetCooldown()
    {
        return tickingCooldown;
    }
}
