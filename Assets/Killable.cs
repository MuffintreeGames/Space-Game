using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public float MaxHealth;    //starting amount of health object has
    private float currentHealth;    //current health of object. When this becomes 0, object dies
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);   //can use OnDestroy() in companion scripts to do object-specific behaviour
        }
    }
}
