using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    public int Damage;  //amount of damage inflicted on contact
    public LayerMask TargetLayers;  //layers that should be hit by attack
    public LayerMask DamagedLayers; //layers that should take damage from attack
    public bool DestroyOnHit;   //should this object be removed on hitting something or persist
    public bool RepeatedDamage;   //if true, targets will take damage as long as they are in contact with attack; otherwise, they can only be hit once
    public bool OverTime = false;   //if true, object is designed to do damage over time. Otherwise, damage is meant to be done in single large burst
    public float InvincibilityDuration = 0f;    //length of invincibility granted
    public bool BelongsToGoliath = false;   //if true, give Goliath exp when this kills something

    protected float damageMultiplier = 1f;    //multiplier to apply to damage

    protected Dictionary<GameObject, bool> hitTargets = new Dictionary<GameObject, bool>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollisionStay2D(Collision2D col)  //uncomment this if you want to use a collider rather than a trigger for a hitbox at some point
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
                    return;
                }
                hitTargets.Add(hitGameObject, true);
            }

            if ((DamagedLayers & (1 << hitGameObject.layer)) != 0)
            {
                targetKillable.TakeDamage(Damage * damageMultiplier, BelongsToGoliath, InvincibilityDuration);
                Debug.Log("inflicting " + Damage * damageMultiplier + " damage!");
            }
            if (DestroyOnHit)
            {
                if (PhotonNetwork.IsConnected)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                } else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    protected void OnTriggerStay2D(Collider2D col)
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
                if (PhotonNetwork.IsConnected)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void ClearHitTargets()   //call to empty out the hit targets list. Useful when an attack has finished to signal a new attack can happen
    {
        hitTargets.Clear();
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        damageMultiplier *= multiplier;
    }

    public void RemoveDamageMultiplier(float multiplier)
    {
        damageMultiplier /= multiplier;
    }
}
