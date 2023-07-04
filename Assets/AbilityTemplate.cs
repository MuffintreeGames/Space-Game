using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Photon.Pun;

public abstract class AbilityTemplate : MonoBehaviour
{

    public enum AbilityCategory { Attack, Dash, Projectile, Buff, Defense};

    public float cooldown;  //time between ability uses
    public Sprite icon;  //symbol for the ability
    public string displayName;  //name shown in UI

    protected GoliathController parentGoliath;
    protected float tickingCooldown;
    protected bool offCooldown = true;

    protected bool listenersAttached = false;

    protected AbilityCategory abilityType;    //should be chosen by the class that inherits this

    public int numOfCopies = 0;

    public int ID;

    protected void InitializeAbility(AbilityCategory abilityType)
    {
        parentGoliath = GetComponent<GoliathController>();
        UpgradeSelf(1);
        enabled = false;
        this.abilityType = abilityType;
        SceneManager.sceneLoaded += ResetListeners;
    }

    public AbilityCategory GetAbilityType()
    {
        return abilityType;
    }

    // Update is called once per frame
    protected void ManageCooldown()
    {
        if (!listenersAttached)
        {
            if (parentGoliath)
            {
                Debug.Log("listeners attaching!");
                GoliathController.GoliathLevelup.AddListener(UpgradeSelf);
                GoliathController.GoliathFinishAttack.AddListener(ContinueCombo);
                listenersAttached = true;
            }
        }

        if (!offCooldown) {
            tickingCooldown -= Time.deltaTime;
            if (tickingCooldown <= 0f)
            {
                offCooldown = true;
            }
        }
    }

    protected bool PrepareToUseAbility()    //check if ability can be used, start cooldown
    {
        if (!enabled) {   //if for whatever reason the ability has been used without being activated properly, just activate it
            enabled = true;
        }

        if (!IsOffCooldown())
        {
            Debug.Log("ability not off cooldown!");
            return false;
        }

        if (!parentGoliath.StartAbility(this))
        {
            Debug.Log("goliath doing something else!");
            return false;
        }

        offCooldown = false;
        tickingCooldown = cooldown;
        return true;
    }

    protected void PrepareToEndAbility()    //tell goliath to stop ability
    {
        parentGoliath.EndAbility(this);
    }

    public void UseAbility()
    {
        if (!PrepareToUseAbility())
        {
            return;
        }

        if (numOfCopies > 1)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonView.Get(this).RPC("RpcUseEvolvedAbility", RpcTarget.All, displayName);
            }
            else
            {
                UseEvolvedAbility();
            }
        } else
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonView.Get(this).RPC("RpcUseNormalAbility", RpcTarget.All, displayName);
            } else
            {
                UseNormalAbility();
            }
        }
    }

    private void ResetListeners(Scene scene, LoadSceneMode mode)   //needed in order to reattach listeners on a scene change
    {
        listenersAttached = false;
    }

    public virtual void UseNormalAbility()
    {
        
    }

    [PunRPC]
    public void RpcUseNormalAbility(string abilityName)
    {
        if (abilityName == displayName) //needed to avoid calling every ability simultaneously
        {
            if (!enabled)
            {   //if for whatever reason the ability has been used without being activated properly, just activate it
                enabled = true;
            }
            UseNormalAbility();
        }
    }

    public void UseEvolvedAbility() //by default, halve the cooldown
    {
        UseNormalAbility();
        tickingCooldown = tickingCooldown / 2;
    }

    [PunRPC]
    public void RpcUseEvolvedAbility(string abilityName)
    {
        if (abilityName == displayName) //needed to avoid calling every ability simultaneously
        {
            UseEvolvedAbility();
        }
    }

    [PunRPC]
    public void SetAction1(string abilityName)
    {
        if (abilityName != displayName)
        {
            return;
        }
        parentGoliath.Action1.numOfCopies -= 1;
        parentGoliath.Action1.DisableAbility();
        parentGoliath.Action1 = this;
        parentGoliath.Action1.numOfCopies += 1;
    }

    [PunRPC]
    public void SetAction2(string abilityName)
    {
        if (abilityName != displayName)
        {
            return;
        }
        parentGoliath.Action2.numOfCopies -= 1;
        parentGoliath.Action2.DisableAbility();
        parentGoliath.Action2 = this;
        parentGoliath.Action2.numOfCopies += 1;
    }

    [PunRPC]
    public void SetAction3(string abilityName)
    {
        if (abilityName != displayName)
        {
            return;
        }
        parentGoliath.Action3.numOfCopies -= 1;
        parentGoliath.Action3.DisableAbility();
        parentGoliath.Action3 = this;
        parentGoliath.Action3.numOfCopies += 1;
    }

    [PunRPC]
    public void SetAction4(string abilityName)
    {
        if (abilityName != displayName)
        {
            return;
        }
        parentGoliath.Action4.numOfCopies -= 1;
        parentGoliath.Action4.DisableAbility();
        parentGoliath.Action4 = this;
        parentGoliath.Action4.numOfCopies += 1;
    }

    public void DisableAbility()
    {
        CancelAbility();
        tickingCooldown = 0f;
        enabled = false;
    }

    public void IsEvolved() //abilities are evolved if you have more than one copy of them
    {

    }

    public virtual void CancelAbility()
    {

    }

    protected virtual void ContinueCombo()
    {

    }

    public virtual void UpgradeSelf(int goliathLevel)
    {

    }

    public bool IsOffCooldown()
    {
        return offCooldown;
    }

    public float GetCooldown()
    {
        return tickingCooldown;
    }
}
