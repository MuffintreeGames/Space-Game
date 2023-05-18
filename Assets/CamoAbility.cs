using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamoAbility : BuffAbility  //turn invisible; goliath player can still see self, god can't
{
    public SpriteRenderer goliathRenderer;
    public float transparency = 0.5f;

    private float currentDuration = 0f;
    private bool currentlyInvisible = false;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Buff);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyInvisible)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= effectDuration)
            {
                CancelAbility();
            }
        }
    }

    public override void UseNormalAbility()
    {
        currentDuration = 0f;
        currentlyInvisible = true;
        if (RoleManager.isGoliath)
        {
            goliathRenderer.color = new Color(goliathRenderer.color.r, goliathRenderer.color.g, goliathRenderer.color.b, transparency);
        } else
        {
            goliathRenderer.color = new Color(goliathRenderer.color.r, goliathRenderer.color.g, goliathRenderer.color.b, 0f);
        }
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyInvisible = false;
        goliathRenderer.color = new Color(goliathRenderer.color.r, goliathRenderer.color.g, goliathRenderer.color.b, 1f);
    }
}
