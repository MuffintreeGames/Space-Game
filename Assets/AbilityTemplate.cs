using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityTemplate : MonoBehaviour
{
    public float cooldown;  //time between ability uses
    public Sprite icon;  //symbol for the ability

    protected GoliathController parentGoliath;
    protected float tickingCooldown;
    protected bool offCooldown = true;

    protected bool listenersAttached = false;

    protected void InitializeAbility()
    {
        Debug.Log(this);
        parentGoliath = GetComponent<GoliathController>();
        UpgradeSelf(1);
        enabled = false;
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

    protected void StartCooldown()
    {
        if (!enabled) {   //if for whatever reason the ability has been used without being activated properly, just activate it
            enabled = true;
        }

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
