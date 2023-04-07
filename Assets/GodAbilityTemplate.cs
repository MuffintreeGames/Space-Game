using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class GodAbilityTemplate : MonoBehaviour
{

    public enum GodAbilityCategory { Attack, Projectile, Buff, Spawn};

    public float cooldown;  //time between ability uses
    public float manaCost;  //amount of mp spent on ability
    public Sprite icon;  //symbol for the ability
    public string displayName;  //name shown in ui
    public bool spendOnClick = false;   //if true, ability costs mp + cooldown on clicking icon; otherwise, will be spent on confirming action

    protected GodController parentGod;

    protected GoliathController enemyGoliath;
    protected float tickingCooldown;
    protected bool offCooldown = true;

    protected bool listenersAttached = false;

    protected GodAbilityCategory abilityType;    //should be chosen by the class that inherits this

    protected void InitializeAbility(GodAbilityCategory abilityType)
    {
        parentGod = GetComponent<GodController>();
        enemyGoliath = GameObject.Find("Goliath").GetComponent<GoliathController>();
        enabled = false;
        this.abilityType = abilityType;
    }

    public GodAbilityCategory GetAbilityType()
    {
        return abilityType;
    }

    // Update is called once per frame
    protected void ManageCooldown()
    {
        if (!listenersAttached)
        {
            if (enemyGoliath)
            {
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

        if (spendOnClick)
        {
            if (!parentGod.SpendMP(manaCost))
            {
                Debug.Log("can't afford mp!");
                return false;
            }
            offCooldown = false;
            tickingCooldown = cooldown;
        }

        return true;
    }

    protected void PrepareToEndAbility()
    {

    }

    public void UseAbility()
    {
        if (!PrepareToUseAbility())
        {
            return;
        }
        UseNormalAbility();
    }

    public virtual void UseNormalAbility()
    {
        
    }

    public void DisableAbility()
    {
        CancelAbility();
        tickingCooldown = 0f;
        enabled = false;
    }

    public virtual void CancelAbility()
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
