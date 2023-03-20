using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public float MaxHealth;    //starting amount of health object has
    public bool DamageTo1 = false;  //if true, object only takes 1 damage from any valid hits
    public bool DamageFlashToggle = true;
    public Color DamageFlash = Color.red;

    private float flashDuration = 0.1f;
    private bool flashing = false;
    private float currentFlashTime = 0f;
    private Color previousColor = Color.white;
    private float currentHealth;    //current health of object. When this becomes 0, object dies
    private EXPSource expScript;
    private GrantAbility abilityScript;
    private SpriteRenderer parentSprite;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        expScript = GetComponent<EXPSource>();
        abilityScript = GetComponent<GrantAbility>();
        parentSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flashing)
        {
            currentFlashTime += Time.deltaTime;
            if (currentFlashTime >= flashDuration)
            {
                flashing = false;
                parentSprite.color = previousColor;
            }
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damage, bool fromGoliath)
    {
        if (DamageTo1)
        {
            currentHealth -= 1;
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            if (fromGoliath && expScript != null)
            {
                expScript.KilledByGoliath();    //used to grant exp to the goliath if killed by something controlled by the goliath
            }

            if (fromGoliath && abilityScript != null)
            {
                abilityScript.KilledByGoliath();    //used to grant ability to the goliath if killed by something controlled by the goliath. Note that objects that grant abilities shouldn't be killable by non-goliath things
            }
            Destroy(this.gameObject);
        }

        if (!flashing) {
            previousColor = parentSprite.color;
            parentSprite.color = DamageFlash;
        }

        currentFlashTime = 0f;
        flashing = true;
    }
}
