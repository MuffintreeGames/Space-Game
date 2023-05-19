using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerAbility : AbilityTemplate  //fire several pellets which hover in place
{
    public float baseProjectileSpeed = 20f;
    public float projectileDuration = 0.2f;
    public int baseProjectileDamage = 10;    //base damage of attack
    public int damagePerLevel = 5;   //extra damage per goliath level
    public GameObject GoliathShot;  //game object to be used as projectile

    public float duration = 3f;  //time flamethrower is used for
    public float timeBetweenShots = 0.1f;
    public int shotsPerSweep = 5;    //shots done during a single left-to-right/right-to-left sweep
    public float spread = 10f;  //how many degrees the projectiles will be spread across

    private bool firing = false;
    private float fireTimeElapsed = 0f;
    private float timeUntilShot = 0f;
    private int shotsFired = 0; //used to track point in shot spread we're at

    private float projectileSpeed;
    private Vector3 projectileOffset;   //used to keep projectiles spawning around the head area
    //private float deceleration;
    //private float decelVariance;
    

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Attack);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (firing)
        {
            fireTimeElapsed += Time.deltaTime;
            if (fireTimeElapsed > duration)
            {
                CancelAbility();
                return;
            }
            timeUntilShot -= Time.deltaTime;
            if (timeUntilShot < 0 )
            {
                timeUntilShot = timeBetweenShots;
                GameObject firedShot;
                if (PhotonNetwork.IsConnected) firedShot = PhotonNetwork.Instantiate(GoliathShot.name, parentGoliath.transform.position, Quaternion.identity);
                else firedShot = Instantiate(GoliathShot, parentGoliath.transform.position + (Quaternion.AngleAxis(parentGoliath.transform.eulerAngles.z, Vector3.forward) * projectileOffset), Quaternion.identity);
                int adjustedShotsFired = shotsFired;
                if (adjustedShotsFired >= shotsPerSweep)
                {
                    adjustedShotsFired = shotsPerSweep - (adjustedShotsFired - shotsPerSweep);  //results in firing in the opposite order of before
                }
                float shotAngle = parentGoliath.transform.eulerAngles.z + ((adjustedShotsFired - shotsPerSweep / 2) * spread);
                firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, shotAngle, projectileDuration);
                shotsFired += 1;
                if (shotsFired >= (shotsPerSweep * 2))
                {
                    shotsFired = 0;
                }
            }
        }
    }

    public override void UseNormalAbility()
    {
        firing = true;
        fireTimeElapsed = 0f;
        timeUntilShot = 0f;
        shotsFired = 0;
        parentGoliath.StartComboAttack();
    }

    public override void CancelAbility()
    {
        firing = false;
        parentGoliath.StopComboAttack();
        parentGoliath.EndAbility(this);
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        AttackObject attackScript = GoliathShot.GetComponent<AttackObject>();
        attackScript.Damage = baseProjectileDamage + (goliathLevel * damagePerLevel);
        attackScript.DamagedLayers = parentGoliath.damagableLayers;
        projectileSpeed = baseProjectileSpeed * goliathLevel;
        switch (goliathLevel)
        {
            case 1:
                GoliathShot.transform.localScale = new Vector3(1f, 1f, 1f);
                projectileOffset = new Vector3(0f, 0.5f);
                break;
            case 2:
                GoliathShot.transform.localScale = new Vector3(2f, 2f, 2f);
                projectileOffset = new Vector3(0f, 1f);
                break;
            case 3:
                GoliathShot.transform.localScale = new Vector3(3f, 3f, 3f);
                projectileOffset = new Vector3(0f, 1.5f);
                break;
            case 4:
                GoliathShot.transform.localScale = new Vector3(4f, 4f, 4f);
                projectileOffset = new Vector3(0f, 2f);
                break;
            case 5:
                GoliathShot.transform.localScale = new Vector3(5f, 5f, 5f);
                projectileOffset = new Vector3(0f, 2.5f);
                break;
        }
    }
}
