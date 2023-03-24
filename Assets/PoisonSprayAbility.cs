using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSprayAbility : AbilityTemplate  //fire several pellets which hover in place
{
    public float projectileSpeed = 15f;
    public float projectileDuration = 3f;
    public int baseProjectileDamage = 10;    //base damage of attack
    public int damagePerLevel = 5;   //extra damage per goliath level
    public GameObject GoliathShot;  //game object to be used as projectile

    public float projectileCount = 11f;  //number of shots to fire
    public float deceleration = 10f;    //how quickly the projectiles should stop
    public float decelVariance = 2f;    //how much deceleration could vary
    public float spread = 10f;  //how many degrees the projectiles will be spread across
    

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

        for (int n = 0; n < projectileCount; n++)
        {
            GameObject firedShot = Instantiate(GoliathShot, parentGoliath.transform.position, Quaternion.identity);
            float shotAngle = parentGoliath.transform.eulerAngles.z + ((n - projectileCount / 2) * spread);
            float randomDecel = deceleration + Random.Range(-decelVariance, decelVariance);
            firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, shotAngle, projectileDuration, -randomDecel, true);
        }
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
