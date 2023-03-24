using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAbility : AbilityTemplate  //fire a projectile
{
    public float projectileSpeed = 20f;
    public float projectileDuration = 3f;
    public int baseProjectileDamage = 10;    //base damage of attack
    public int damagePerLevel = 5;   //extra damage per goliath level
    public GameObject GoliathShot;  //game object to be used as projectile

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Projectile);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }

    public override void UseAbility()
    {
        if (!PrepareToUseAbility())
        {
            return;
        }

        GameObject firedShot = Instantiate(GoliathShot, parentGoliath.transform.position, Quaternion.identity);
        Debug.Log("checking shot damage: " + firedShot.GetComponent<AttackObject>().Damage);
        firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        AttackObject attackScript = GoliathShot.GetComponent<AttackObject>();
        attackScript.Damage = baseProjectileDamage + (goliathLevel * damagePerLevel);
        switch (goliathLevel)
        {
            case 2:
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize2"));
                break;
            case 3:
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize3"));
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel1"));
                break;
            case 4:
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize4"));
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel2"));
                break;
            case 5:
                attackScript.DamagedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel3"));
                break;
        }
    }
}
