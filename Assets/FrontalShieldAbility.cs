using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontalShieldAbility : AbilityTemplate   //destroy incoming projectiles from the front; physical attacks go through
{
    public float shieldDuration = 1.5f;

    public GameObject shield;
    private float currentShieldTime = 0f;
    private bool shieldActive = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializeAbility(AbilityCategory.Defense);
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldown();
        if (shieldActive)
        {
            currentShieldTime -= Time.deltaTime;
            if (currentShieldTime < 0)
            {
                shield.SetActive(false);
                shieldActive = false;
            }
        }
    }

    public override void UseNormalAbility()
    {
        shield.SetActive(true);
        shieldActive = true;
        currentShieldTime = shieldDuration;
    }
}
