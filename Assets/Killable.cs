using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public float MaxHealth;    //starting amount of health object has
    public bool DamageTo1 = false;  //if true, object only takes 1 damage from any valid hits
    public bool DamageFlashToggle = true;
    public Color DamageFlash = Color.red;
    public float InvincibilityOpacity = 0.5f;

    private float flashDuration = 0.1f;
    private bool flashing = false;
    private float currentFlashTime = 0f;
    private Color previousColor = Color.white;

    private bool invincible = false;
    private float invincibilityTime = 0f;   //how long we can remain invincible total
    private float currentInvincibilityTime = 0f;    //how long we have been invincible already

    private float currentHealth;    //current health of object. When this becomes 0, object dies
    private EXPSource expScript;
    private GrantAbility abilityScript;
    private SpriteRenderer parentSprite;

    private float damageMultiplier = 1f;

    public int blockableHits = 0;   //how many hits this unit can block before taking damage

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

        if (invincible)
        {
            currentInvincibilityTime += Time.deltaTime;
            if (currentInvincibilityTime >= invincibilityTime)
            {
                invincible = false;
                parentSprite.color = new Color(parentSprite.color.r, parentSprite.color.g, parentSprite.color.b);
            } else
            {
                parentSprite.color = new Color(parentSprite.color.r, parentSprite.color.g, parentSprite.color.b, InvincibilityOpacity);
            }
        }
    }

    public void ApplyDamageMultiplier(float multiplier)
    {
        damageMultiplier = damageMultiplier * multiplier;
    }

    public void RemoveDamageMultiplier(float multiplier)
    {
        damageMultiplier = damageMultiplier / multiplier;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public bool IsInvincible()
    {
        return invincible;
    }

    public void AddBlock(int numHits)
    {
        blockableHits += numHits;
    }

    public int CheckBlock()
    {
        return blockableHits;
    }

    public void TakeDamage(float damage, bool fromGoliath, float invincibilityDuration)
    {
        if (invincible)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        if (blockableHits > 0)
        {
            blockableHits -= 1;
            return;
        }

        if (DamageTo1)
        {
            currentHealth -= 1;
        }
        else
        {
            currentHealth -= damage * damageMultiplier;
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

        if (invincibilityDuration > 0f)
        {
            invincibilityTime = invincibilityDuration;
            currentInvincibilityTime = 0f;
            invincible = true;
        }
    }
}
