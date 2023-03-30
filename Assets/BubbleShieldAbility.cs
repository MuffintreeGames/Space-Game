using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldAbility : AbilityTemplate   //negates one hit before disappearing; ineffective against repeated hits
{
    public float shieldDuration = 4f;

    public BubbleShieldController shield;
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
    }

    public override void UseNormalAbility()
    {
        shield.ActivateShield(shieldDuration, 1);
        shieldActive = true;
        currentShieldTime = shieldDuration;
    }
}
