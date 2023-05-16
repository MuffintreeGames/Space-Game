using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAbility : AbilityTemplate  //long range, piercing, low damage attack
{
    public float baseProjectileSpeed = 5f;
    public float projectileDuration = 3f;
    public int baseProjectileDamage = 1;    //base damage of attack
    public int damagePerLevel = 1;   //extra damage per goliath level
    public GameObject Laser;  //game object to be used as projectile
    private AttackObject LaserScript;

    private bool performingLaser = false;
    private float projectileSpeed;
    private float projectileTime = 0f;
    private float laserLength = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Laser = GameObject.Find("LaserContainer");
        LaserScript = Laser.GetComponentInChildren<AttackObject>();
        base.InitializeAbility(AbilityCategory.Projectile);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (performingLaser)
        {
            projectileTime += Time.deltaTime;
            if (projectileTime > projectileDuration)
            {
                CancelAbility();
            } else
            {
                ContinueLaser();
            }
        }
    }

    public override void UseNormalAbility()
    {
        parentGoliath.StartComboAttack();
        Laser.SetActive(true);
        projectileTime = 0f;
        laserLength = 0f;
        performingLaser = true;

        ContinueLaser();
    }

    private void ContinueLaser()
    {
        laserLength += Time.deltaTime * projectileSpeed;
        Laser.transform.localScale = new Vector2(Laser.transform.localScale.x, laserLength);
    }

    public override void CancelAbility()
    {
        base.CancelAbility();
        parentGoliath.StopComboAttack();
        Laser.SetActive(false);
        performingLaser = false;
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        LaserScript.Damage = baseProjectileDamage + (goliathLevel * damagePerLevel);
        LaserScript.DamagedLayers = parentGoliath.damagableLayers;
        projectileSpeed = baseProjectileSpeed + (1f * goliathLevel);
        switch (goliathLevel)
        {
            case 1:
                Laser.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case 2:
                Laser.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case 3:
                Laser.transform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case 4:
                Laser.transform.localScale = new Vector3(4f, 4f, 4f);
                break;
            case 5:
                Laser.transform.localScale = new Vector3(5f, 5f, 5f);
                break;
        }
    }
}
