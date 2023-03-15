using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public float MaxHealth;    //starting amount of health object has
    private float currentHealth;    //current health of object. When this becomes 0, object dies
    private EXPSource expScript;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        expScript = GetComponent<EXPSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damage, bool fromGoliath)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (fromGoliath && expScript != null)
            {
                expScript.KilledByGoliath();    //used to grant exp to the goliath if killed by something controlled by the goliath
            }
            Destroy(this.gameObject);
        }
    }
}
