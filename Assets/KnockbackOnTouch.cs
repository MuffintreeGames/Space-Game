using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnTouch : MonoBehaviour
{
    public float knockback;  //force of knockback
    public LayerMask BouncedLayers; //layers that should be affected

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnCollisionEnter2D(Collision2D col)  //uncomment this if you want to use a collider rather than a trigger for a hitbox at some point
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

            Vector2 direction = (Vector2)hitGameObject.transform.position - col.contacts[0].point;
            Debug.Log("knockback in " + direction);
            targetRigid.AddForce(direction * knockback, ForceMode2D.Impulse);
        }
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
