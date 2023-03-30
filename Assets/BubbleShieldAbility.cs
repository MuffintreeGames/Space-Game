using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldAbility : BuffAbility   //negates one hit before disappearing; ineffective against repeated hits
{

    public BubbleShieldController shield;
    // Start is called before the first frame update
    void Start()
    {
        InitializeAbility(AbilityCategory.Buff);
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldown();
    }

    public override void UseNormalAbility()
    {
        shield.ActivateShield(effectDuration, 1);
    }
}
