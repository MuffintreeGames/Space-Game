using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatPlanetAbility : AbilityTemplate  //melee attack on planet; if it would kill, then after a moment shoot out rubble shotgun
{
    public float baseProjectileSpeed = 20f;
    public float projectileDuration = 3f;
    public float biteDuration = 0.2f;   //time for bite hitbox to be active
    public int baseBiteDamage = 20;
    public int biteDamagePerLevel = 10;
    public float chewDuration = 0.3f;   //time between bite and shot
    public int baseProjectileDamage = 10;    //base damage of attack
    public int projectileDamagePerLevel = 5;   //extra damage per goliath level
    public GameObject BiteHitbox;   //object used for initial bite
    public GameObject SprayShot;  //game object to be used as sprayed projectile

    private bool attacking = false;
    private bool biting = false;   //in bite part of attack
    private float biteTimeElapsed = 0f;

    private bool chewing = false;
    private float chewTimeElapsed = 0f;

    public float projectileCount = 11f;  //number of shots to fire
    //public float baseDeceleration = 10f;    //how quickly the projectiles should stop
    //public float baseDecelVariance = 2f;    //how much deceleration could vary
    public float spread = 1f;  //how many degrees the projectiles will be spread across

    private float projectileSpeed;
    //private float deceleration;
    //private float decelVariance;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Attack);
        EatOnKill.AtePlanet.AddListener(ChewUp);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (attacking)
        {
            if (biting)
            {
                biteTimeElapsed += Time.deltaTime;
                if (biteTimeElapsed > biteDuration)
                {
                    Debug.Log("disabling hitbox!");
                    CancelAbility();
                }
            } else if (chewing)
            {
                chewTimeElapsed += Time.deltaTime;
                if (chewTimeElapsed > chewDuration)
                {
                    Debug.Log("firing!");
                    SpitUp();
                }
            }
        }
    }

    public override void UseNormalAbility()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        parentGoliath.StartComboAttack();
        BiteHitbox.SetActive(true);
        biting = true;
        attacking = true;
        biteTimeElapsed = 0f;
        /*GameObject firedShot;
        if (PhotonNetwork.IsConnected) firedShot = PhotonNetwork.Instantiate(SprayShot.name, parentGoliath.transform.position, Quaternion.identity);
        else firedShot = Instantiate(SprayShot, parentGoliath.transform.position, Quaternion.identity);
        Debug.Log("checking shot damage: " + firedShot.GetComponent<AttackObject>().Damage);
        firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);*/
    }

    private void ChewUp(bool filler)    //parameter is unused
    {
        BiteHitbox.SetActive(false);
        biting = false;
        chewing = true;
        chewTimeElapsed = 0f;
    }

    private void SpitUp()
    {
        chewing = false;
        for (int n = 0; n < projectileCount; n++)
        {
            GameObject firedShot;
            if (PhotonNetwork.IsConnected) firedShot = PhotonNetwork.Instantiate(SprayShot.name, parentGoliath.transform.position, Quaternion.identity);
            else firedShot = Instantiate(SprayShot, parentGoliath.transform.position, Quaternion.identity);
            float shotAngle = parentGoliath.transform.eulerAngles.z + ((n - projectileCount / 2) * spread);
            //float randomDecel = deceleration + Random.Range(-decelVariance, decelVariance);
            firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, shotAngle, projectileDuration);
        }
        CancelAbility();
    }

    public override void CancelAbility()
    {
        base.CancelAbility();
        BiteHitbox.SetActive(false);
        biting = false;
        chewing = false;
        attacking = false;
        parentGoliath.StopComboAttack();
        parentGoliath.EndAbility(this);
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        EatOnKill biteAttackScript = BiteHitbox.GetComponent<EatOnKill>();
        biteAttackScript.Damage = baseProjectileDamage + (goliathLevel * biteDamagePerLevel);
        biteAttackScript.DamagedLayers = parentGoliath.damagableLayers;

        AttackObject sprayAttackScript = SprayShot.GetComponent<AttackObject>();
        sprayAttackScript.Damage = baseProjectileDamage + (goliathLevel * projectileDamagePerLevel);
        sprayAttackScript.DamagedLayers = parentGoliath.damagableLayers;
        projectileSpeed = baseProjectileSpeed + (1f * goliathLevel);
        switch (goliathLevel)
        {
            case 1:
                SprayShot.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case 2:
                SprayShot.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case 3:
                SprayShot.transform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case 4:
                SprayShot.transform.localScale = new Vector3(4f, 4f, 4f);
                break;
            case 5:
                SprayShot.transform.localScale = new Vector3(5f, 5f, 5f);
                break;
        }
    }
}
