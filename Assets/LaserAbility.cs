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
    private BlockableLaser LaserScript;

    private bool performingLaser = false;
    private float projectileSpeed;
    private float projectileTime = 0f;
    private float laserLength = 0f;
    private bool skipFrame = false; //stops beam from growing if it was blocked last frame
    //private float currentMinDistance = 

    // Start is called before the first frame update
    void Start()
    {
        //Laser = GameObject.Find("LaserContainer");
        LaserScript = Laser.GetComponentInChildren<BlockableLaser>();
        base.InitializeAbility(AbilityCategory.Projectile);
        BlockableLaser.LaserBlocked.AddListener(AdjustBlockedLaser);
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
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }
        laserLength += Time.deltaTime * projectileSpeed;
        Laser.transform.localScale = new Vector2(Laser.transform.localScale.x, laserLength);
    }

    private void AdjustBlockedLaser(Vector2 blockingCoords)  //if there's a large enough obstacle touching the laser, it should get blocked. If multiple such objects exist, only the one closest to the goliath should work
    {
        //Vector2 currentGrappleTarget = (Vector2)grappledObject.transform.position + relativeGrapplePoint;
        Vector2 targetDirection = blockingCoords - (Vector2)Laser.transform.position;
        float currentDistance = Mathf.Sqrt(targetDirection.x * targetDirection.x + targetDirection.y * targetDirection.y);
        if (laserLength > currentDistance)
        {
            laserLength = currentDistance;
            Laser.transform.localScale = new Vector2(Laser.transform.localScale.x, laserLength);
            skipFrame = true;
        }
        //float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        //goliathTongue.transform.rotation = targetRotation;
        //goliathTongue.transform.localScale = new Vector3(goliathTongue.transform.localScale.x, currentDistance / goliathTransform.localScale.y, goliathTongue.transform.localScale.z);
        //goliathRigid.velocity = targetDirection.normalized * grappleSpeed / slowComponent.GetSlowFactor();
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
                LaserScript.blockingLayers &= ~(1 << LayerMask.NameToLayer("DestructibleSize1"));
                break;
            case 3:
                Laser.transform.localScale = new Vector3(3f, 3f, 3f);
                LaserScript.blockingLayers &= ~(1 << LayerMask.NameToLayer("DestructibleSize2"));
                break;
            case 4:
                Laser.transform.localScale = new Vector3(4f, 4f, 4f);
                LaserScript.blockingLayers &= ~(1 << LayerMask.NameToLayer("DestructibleSize3"));
                break;
            case 5:
                Laser.transform.localScale = new Vector3(5f, 5f, 5f);
                LaserScript.blockingLayers &= ~(1 << LayerMask.NameToLayer("DestructibleSize4"));
                break;
        }
    }
}
