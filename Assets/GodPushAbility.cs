using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodPushAbility : GodAbilityTemplate //create wave of force
{
    public GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(GodAbilityCategory.Attack);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }

    public override void UseNormalAbility()
    {
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject newCrosshair = Instantiate(crosshair, mouseCoords, Quaternion.identity);
        newCrosshair.GetComponent<GodChoosePushTarget>().parentAbility = this;
    }
}
