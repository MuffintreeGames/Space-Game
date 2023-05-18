using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EatOnKill : AttackObject
{
    public class AtePlanetEvent : UnityEvent<bool>
    {

    }

    public static AtePlanetEvent AtePlanet;

    private void Awake()
    {
        if (AtePlanet == null)
        {
            AtePlanet = new AtePlanetEvent();
        }
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    new protected void OnTriggerStay2D(Collider2D col)
    {
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = col.gameObject;

        if ((TargetLayers & (1 << hitGameObject.layer)) != 0)
        {
            Killable targetKillable = hitGameObject.GetComponent<Killable>();
            if (targetKillable == null)
            {
                return;
            }

            if (targetKillable.IsInvincible())
            {
                return;
            }

            if (!RepeatedDamage)
            {
                if (hitTargets.ContainsKey(hitGameObject))
                {
                    //Debug.Log("have already hit this object before");
                    return;
                }
                hitTargets.Add(hitGameObject, true);
            }

            if ((DamagedLayers & (1 << hitGameObject.layer)) != 0)
            {
                if (targetKillable.GetHealth() <= Damage)   //special part for EatOnKill: trigger AtePlanetEvent if target will die from hit
                {
                    AtePlanet.Invoke(true);
                }
                if (OverTime)
                {
                    targetKillable.TakeDamage(Damage * Time.deltaTime, BelongsToGoliath, 0f);
                }
                else
                {
                    targetKillable.TakeDamage(Damage, BelongsToGoliath, InvincibilityDuration);
                }
            }
            if (DestroyOnHit)
            {
                Destroy(this.gameObject);
            }
        }
    }

    new protected void OnCollisionStay2D(Collision2D collision)
    {
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = collision.gameObject;
        if ((TargetLayers & (1 << hitGameObject.layer)) != 0)
        {
            Killable targetKillable = hitGameObject.GetComponent<Killable>();
            if (targetKillable == null)
            {
                return;
            }

            if (targetKillable.IsInvincible())
            {
                return;
            }

            if (!RepeatedDamage)
            {
                if (hitTargets.ContainsKey(hitGameObject))
                {
                    return;
                }
                hitTargets.Add(hitGameObject, true);
            }

            if ((DamagedLayers & (1 << hitGameObject.layer)) != 0)
            {
                if (targetKillable.GetHealth() <= Damage)   //special part for EatOnKill: trigger AtePlanetEvent if target will die from hit
                {
                    AtePlanet.Invoke(true);
                }
                targetKillable.TakeDamage(Damage * damageMultiplier, BelongsToGoliath, InvincibilityDuration);
                Debug.Log("inflicting " + Damage * damageMultiplier + " damage!");
            }
            if (DestroyOnHit)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
