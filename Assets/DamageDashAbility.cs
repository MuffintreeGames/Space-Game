using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDashAbility : DashAbility
{

    public int baseDamage = 30;
    public int damagePerLevel = 5;

    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Dash);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyDashing)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= dashDuration)
            {
                Debug.Log("ending dash");
                CancelAbility();
            }
        }
    }

    public override void UseNormalAbility()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        base.UseNormalAbility();
        if (currentlyDashing)   //used to check for success in activating ability
        {
            parentGoliath.ActivateRamHitbox(damage);
        }
    }

    public override void CancelAbility()
    {
        base.CancelAbility();
        parentGoliath.DisableRamHitbox();
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        damage = baseDamage + (goliathLevel * damagePerLevel);
    }
}
