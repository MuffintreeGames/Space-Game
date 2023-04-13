using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodController : MonoBehaviour
{
    public float maxMp = 100f;
    public float mpRegen = 5f;  //mp regained per second
    public float mpRegenBonus = 1f; //extra mp regen based on the number of barriers destroyed by the goliath

    private float currentMp = 100f;
    private float currentMPRegen;

    public GodAbilityTemplate Action1; //ability tied to the action1 button
    public GodAbilityTemplate Action2; //ability tied to the action2 button
    public GodAbilityTemplate Action3; //ability tied to the action3 button
    public float Action3UnlockTime = 100f;   //time until action3 is unlocked
    public GodAbilityTemplate Action4; //ability tied to the action4 button
    public float Action4UnlockTime = 200f;   //time until action4 is unlocked
    public GodAbilityTemplate Action5; //ability tied to the action5 button
    public float Action5UnlockTime = 300f;   //time until action5 is unlocked
    public GodAbilityTemplate Action6; //ability tied to the action6 button
    public GodAbilityTemplate Action7; //ability tied to the action7 button

    private bool canUseAbilities = true;
    // Start is called before the first frame update
    void Start()
    {
        SectorWall.UnlockSector.AddListener(IncreaseMPRegen);
        currentMPRegen = mpRegen;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMp < maxMp)
        {
            currentMp += Time.deltaTime * currentMPRegen;
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
        if (CanUseAbility1())
        {
            Action1.UseAbility();
        }
    }

    public bool CanUseAbility1()
    {
        return Action1 != null && canUseAbilities && currentMp >= Action1.manaCost && Action1.IsOffCooldown();
    }

    public void UseAbility2()
    {
        if (CanUseAbility2())
        {
            Action2.UseAbility();
        }
    }

    public bool CanUseAbility2()
    {
        return Action2 != null && canUseAbilities && currentMp >= Action2.manaCost && Action2.IsOffCooldown();
    }

    public void UseAbility3()
    {
        if (CanUseAbility3())
        {
            Action3.UseAbility();
        }
    }

    public bool CanUseAbility3()
    {
        return Action3 != null && canUseAbilities && currentMp >= Action3.manaCost && SpaceManager.GetElapsedTime() >= Action3UnlockTime && Action3.IsOffCooldown();
    }

    public void UseAbility4()
    {
        if (CanUseAbility4())
        {
            Action4.UseAbility();
        }
    }

    public bool CanUseAbility4()
    {
        return Action4 != null && canUseAbilities && currentMp >= Action4.manaCost && SpaceManager.GetElapsedTime() >= Action4UnlockTime && Action4.IsOffCooldown();
    }

    public void UseAbility5()
    {
        if (CanUseAbility5())
        {
            Action5.UseAbility();
        }
    }

    public bool CanUseAbility5()
    {
        return Action5 != null && canUseAbilities && currentMp >= Action5.manaCost && SpaceManager.GetElapsedTime() >= Action5UnlockTime && Action5.IsOffCooldown();
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

    void IncreaseMPRegen(int x, int y)  //triggered when opening a new sector. parameters are irrelevant, but necessary to avoid errors
    {
        currentMPRegen += mpRegenBonus;
    }
}
