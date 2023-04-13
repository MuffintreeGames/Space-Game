using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMakeEyeAbility : GodAbilityTemplate    //create an eye which gradually paralyzes the goliath if it looks at the eye
{
    public GameObject spawnOutline;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(GodAbilityCategory.Spawn);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
    }

    public override void UseNormalAbility()
    {
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject createdOutline = Instantiate(spawnOutline, mouseCoords, Quaternion.identity);
        createdOutline.GetComponent<PlaceOnClick>().parentAbility = this;
    }
}
