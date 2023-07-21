using Photon.Pun;
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

    GameObject spawnedShot = null;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Projectile);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (spawnedShot == null ) {
            altModeActive = false;
        }
    }

    public override void UseNormalAbility()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            object[] instantiationData = new object[3];
            instantiationData[0] = projectileSpeed;
            instantiationData[1] = parentGoliath.transform.eulerAngles.z;
            instantiationData[2] = projectileDuration;
            spawnedShot = PhotonNetwork.Instantiate(ExplosiveShot.name, parentGoliath.transform.position, Quaternion.identity, 0, instantiationData);
            return;
        }
        else
        {
            GameObject firedShot = Instantiate(ExplosiveShot, parentGoliath.transform.position, Quaternion.identity);
            Debug.Log("checking shot damage: " + firedShot.GetComponent<AttackObject>().Damage);
            firedShot.GetComponent<ExplosiveProjectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);
            spawnedShot = firedShot;
        }
    }

    public override void UseAltMode()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        altModeActive = false;
        if (spawnedShot == null)
        {
            return;
        }
        spawnedShot.GetComponent<ExplosiveProjectile>().TriggerExplosion();
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
                ExplosiveShot.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                explosionScript.transform.localScale = new Vector3(9f, 9f, 1f);
                break;
            case 2:
                ExplosiveShot.transform.localScale = new Vector3(1f, 1f, 1f);
                explosionScript.transform.localScale = new Vector3(12f, 12f, 1f);
                break;
            case 3:
                ExplosiveShot.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                explosionScript.transform.localScale = new Vector3(15f, 15f, 1f);
                break;
            case 4:
                ExplosiveShot.transform.localScale = new Vector3(2f, 2f, 1f);
                explosionScript.transform.localScale = new Vector3(18f, 18f, 1f);
                break;
            case 5:
                ExplosiveShot.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
                explosionScript.transform.localScale = new Vector3(21f, 21f, 1f);
                break;
        }
    }
}
