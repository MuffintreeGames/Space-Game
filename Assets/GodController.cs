using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodController : MonoBehaviour
{
    public float maxMp = 100f;
    public float mpRegen = 5f;  //mp regained per second

    private float currentMp = 100f;

    public GodAbilityTemplate Action1; //ability tied to the action1 button
    public GodAbilityTemplate Action2; //ability tied to the action2 button
    public GodAbilityTemplate Action3; //ability tied to the action3 button
    public GodAbilityTemplate Action4; //ability tied to the action4 button
    public GodAbilityTemplate Action5; //ability tied to the action5 button
    public GodAbilityTemplate Action6; //ability tied to the action6 button
    public GodAbilityTemplate Action7; //ability tied to the action7 button

    private bool canUseAbilities = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMp < maxMp)
        {
            currentMp += Time.deltaTime * mpRegen;
            currentMp = Mathf.Min(currentMp, maxMp);
        }
    }

    public float GetMP()
    {
        return currentMp;
    }

    public float GetMaxMP()
    {
        return maxMp;
    }

    public bool SpendMP(float cost)
    {
        if (currentMp < cost)   //not enough mp
        {
            return false;
        }
        currentMp -= cost;
        return true;
    }

    public void SetAbilityUsage(bool allowed)
    {
        canUseAbilities = allowed;
    }

    public void UseAbility1()
    {
        if (Action1 != null && canUseAbilities)
        {
            Action1.UseAbility();
        }
    }

    public void UseAbility2()
    {
        if (Action2 != null && canUseAbilities)
        {
            Action2.UseAbility();
        }
    }

    public void UseAbility3()
    {
        if (Action3 != null && canUseAbilities)
        {
            Action3.UseAbility();
        }
    }

    public void UseAbility4()
    {
        if (Action4 != null && canUseAbilities)
        {
            Action4.UseAbility();
        }
    }

    public void UseAbility5()
    {
        if (Action5 != null && canUseAbilities)
        {
            Action5.UseAbility();
        }
    }

    public void UseAbility6()
    {
        if (Action6 != null && canUseAbilities)
        {
            Action6.UseAbility();
        }
    }

    public void UseAbility7()
    {
        if (Action7 != null && canUseAbilities)
        {
            Action7.UseAbility();
        }
    }
}
