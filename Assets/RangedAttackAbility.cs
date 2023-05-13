using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAbility : AbilityTemplate  //fire a projectile
{
    public float baseProjectileSpeed = 20f;
    public float projectileDuration = 3f;
    public int baseProjectileDamage = 10;    //base damage of attack
    public int damagePerLevel = 5;   //extra damage per goliath level
    public GameObject GoliathShot;  //game object to be used as projectile

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
        GameObject firedShot = PhotonNetwork.Instantiate(GoliathShot.ToString(), parentGoliath.transform.position, Quaternion.identity);
        Debug.Log("checking shot damage: " + firedShot.GetComponent<AttackObject>().Damage);
        firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        AttackObject attackScript = GoliathShot.GetComponent<AttackObject>();
        attackScript.Damage = baseProjectileDamage + (goliathLevel * damagePerLevel);
        attackScript.DamagedLayers = parentGoliath.damagableLayers;
        projectileSpeed = baseProjectileSpeed + (1f * goliathLevel);
        switch (goliathLevel)
        {
            case 1:
                GoliathShot.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case 2:
                GoliathShot.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case 3:
                GoliathShot.transform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case 4:
                GoliathShot.transform.localScale = new Vector3(4f, 4f, 4f);
                break;
            case 5:
                GoliathShot.transform.localScale = new Vector3(5f, 5f, 5f);
                break;
        }
    }
}
