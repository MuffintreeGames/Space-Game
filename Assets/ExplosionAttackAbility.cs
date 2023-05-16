using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttackAbility : AbilityTemplate  //stop moving while charging big powerful explosion
{
    public int baseExplosionDamage = 20;
    public int explosionDamagePerLevel = 10;
    public float timeToExplode = 1f;
    public GameObject Explosion;  //game object to be used as projectile

    private bool preppingExplosion = false;
    private float prepTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Attack);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (preppingExplosion)
        {
            prepTime += Time.deltaTime;
            if (prepTime > timeToExplode)
            {
                PerformExplosion();
            }
        }
    }

    public override void UseNormalAbility()
    {
        prepTime = 0f;
        preppingExplosion = true;
        parentGoliath.StartComboAttack();
        parentGoliath.LockMovement();
    }

    private void PerformExplosion()
    {
        if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(Explosion.name, parentGoliath.transform.position, Quaternion.identity);
        else Instantiate(Explosion, parentGoliath.transform.position, Quaternion.identity);
        CancelAbility();
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        preppingExplosion = false;
        parentGoliath.StopComboAttack();
        parentGoliath.UnlockMovement();
    }

    public override void UpgradeSelf(int goliathLevel)
    {
        AttackObject explosionScript = Explosion.GetComponent<AttackObject>();
        explosionScript.Damage = baseExplosionDamage + (goliathLevel * explosionDamagePerLevel);
        explosionScript.DamagedLayers = parentGoliath.damagableLayers;
        switch (goliathLevel)
        {
            case 1:
                Explosion.transform.localScale = new Vector3(6f, 6f, 6f);
                break;
            case 2:
                Explosion.transform.localScale = new Vector3(12f, 12f, 12f);
                break;
            case 3:
                Explosion.transform.localScale = new Vector3(18f, 18f, 18f);
                break;
            case 4:
                Explosion.transform.localScale = new Vector3(24f, 24f, 24f);
                break;
            case 5:
                Explosion.transform.localScale = new Vector3(30f, 30f, 30f);
                break;
        }
    }
}
