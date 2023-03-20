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

    protected void InitializeAbility()
    {
        parentGoliath = GetComponent<GoliathController>();
        if (parentGoliath)
        {
            GoliathController.GoliathLevelup.AddListener(UpgradeSelf);
        }
        UpgradeSelf(1);
        enabled = false;
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
