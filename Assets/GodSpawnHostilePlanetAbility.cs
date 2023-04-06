using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSpawnHostilePlanetAbility : GodAbilityTemplate
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
        Instantiate(spawnOutline, mouseCoords, Quaternion.identity);
    }
}
