using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttackAbility : AbilityTemplate //do 3 basic attacks in rapid succession
{
    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Attack);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }
}
