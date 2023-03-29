using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public int numOfCopies = 0;

    protected void InitializeAbility(AbilityCategory abilityType)
    {
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
                GoliathController.GoliathFinishAttack.AddListener(ContinueCombo);
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

    public void UseAbility()
    {
        if (!PrepareToUseAbility())
        {
            return;
        }

        if (numOfCopies > 1)
        {
            UseEvolvedAbility();
        } else
        {
            UseNormalAbility();
        }
    }

    public virtual void UseNormalAbility()
    {
        
    }

    public void UseEvolvedAbility() //by default, halve the cooldown
    {
        UseNormalAbility();
        tickingCooldown = tickingCooldown / 2;
    }

    public void DisableAbility()
    {
        CancelAbility();
        tickingCooldown = 0f;
        enabled = false;
    }

    public void IsEvolved() //abilities are evolved if you have more than one copy of them
    {

    }

    public virtual void CancelAbility()
    {

    }

    protected virtual void ContinueCombo()
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
