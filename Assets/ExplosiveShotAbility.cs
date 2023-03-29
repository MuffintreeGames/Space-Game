using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveShotAbility : AbilityTemplate  //fire a projectile that does minor damage, but explodes at a certain range for big damage
{
    public float baseProjectileSpeed = 20f;
    public float projectileDuration = 3f;
    public int baseProjectileDamage = 5;    //base damage of attack
    public int damagePerLevel = 2;   //extra damage per goliath level
    public int baseExplosionDamage = 20;
    public int explosionDamagePerLevel = 10;
    public GameObject ExplosiveShot;  //game object to be used as projectile

    private float projectileSpeed;

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

    public override void UseNormalAbility()
    {
        GameObject firedShot = Instantiate(ExplosiveShot, parentGoliath.transform.position, Quaternion.identity);
        Debug.Log("checking shot damage: " + firedShot.GetComponent<AttackObject>().Damage);
        firedShot.GetComponent<ExplosiveProjectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        AttackObject attackScript = ExplosiveShot.GetComponent<AttackObject>();
        AttackObject explosionScript = ExplosiveShot.GetComponent<ExplosiveProjectile>().Explosion.GetComponent<AttackObject>();
        attackScript.Damage = baseProjectileDamage + (goliathLevel * damagePerLevel);
        attackScript.DamagedLayers = parentGoliath.damagableLayers;
        explosionScript.Damage = baseExplosionDamage + (goliathLevel * explosionDamagePerLevel);
        explosionScript.DamagedLayers = parentGoliath.damagableLayers;
        projectileSpeed = baseProjectileSpeed + (goliathLevel * 0.5f);
        switch (goliathLevel)
        {
            case 1:
                ExplosiveShot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                explosionScript.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                break;
            case 2:
                ExplosiveShot.transform.localScale = new Vector3(1f, 1f, 1f);
                explosionScript.transform.localScale = new Vector3(7f, 7f, 7f);
                break;
            case 3:
                ExplosiveShot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                explosionScript.transform.localScale = new Vector3(10.5f, 10.5f, 10.5f);
                break;
            case 4:
                ExplosiveShot.transform.localScale = new Vector3(2f, 2f, 2f);
                explosionScript.transform.localScale = new Vector3(14f, 14f, 14f);
                break;
            case 5:
                ExplosiveShot.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                explosionScript.transform.localScale = new Vector3(17.5f, 17.5f, 17.5f);
                break;
        }
    }
}
