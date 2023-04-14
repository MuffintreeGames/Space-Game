using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnTouch : MonoBehaviour
{
    public float knockback;  //force of knockback
    public LayerMask BouncedLayers; //layers that should be affected

    private Vector2 oldCoords;

    // Start is called before the first frame update
    void Start()
    {
        oldCoords = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnTriggerEnter2D(Collider2D col)  //uncomment this if you want to use a collider rather than a trigger for a hitbox at some point
    {
        Debug.Log("knockback collision");
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = col.gameObject;
        if ((BouncedLayers & (1 << hitGameObject.layer)) != 0)
        {
            Rigidbody2D targetRigid = hitGameObject.GetComponent<Rigidbody2D>();
            if (targetRigid == null)
            {
                return;
            }

            SpeedAttackObject speedAttackObject = targetRigid.GetComponent<SpeedAttackObject>();
            if (speedAttackObject != null)
            {
                speedAttackObject.enabled = true;
            }

            Vector2 direction = (Vector2) transform.position - oldCoords;
            SlowableObject slowComponent = hitGameObject.GetComponent<SlowableObject>();
            if (slowComponent != null)
            {
                targetRigid.AddForce(direction.normalized * knockback * targetRigid.mass / slowComponent.GetSlowFactor(), ForceMode2D.Impulse);
            } else
            {
                targetRigid.AddForce(direction.normalized * knockback * targetRigid.mass, ForceMode2D.Impulse);
            }
        }
        oldCoords = transform.position;
    }

    /*void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("knockback collision");
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = col.gameObject;
        if ((BouncedLayers & (1 << hitGameObject.layer)) != 0)
        {
            Rigidbody2D targetRigid = hitGameObject.GetComponent<Rigidbody2D>();
            if (targetRigid == null)
            {
                return;
            }

            Vector2 direction = col.contacts[0].point.normalized;
            Debug.Log("knockback in " + direction);
            targetRigid.AddForce(direction * knockback);
        }
    }*/
}
