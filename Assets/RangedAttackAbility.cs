using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAbility : AbilityTemplate  //fire a projectile
{
    public float projectileSpeed = 20f;
    public float projectileDuration = 3f;
    public GameObject GoliathShot;  //game object to be used as projectile

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility();
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }

    public override void UseAbility()
    {
        Debug.Log("trying to shoot");
        base.StartCooldown();

        GameObject firedShot = Instantiate(GoliathShot, parentGoliath.transform.position, Quaternion.identity);
        firedShot.GetComponent<Projectile>().SetProjectileParameters(projectileSpeed, parentGoliath.transform.eulerAngles.z, projectileDuration);
    }
}
