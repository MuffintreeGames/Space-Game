using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleOnTouch : MonoBehaviour //if this hits something on an appropriate layer, pull goliath to target
{
    public LayerMask TargetLayers;
    private GoliathController parentGoliath;
    // Start is called before the first frame update
    void Start()
    {
        parentGoliath = GameObject.Find("Goliath").GetComponent<GoliathController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = col.gameObject;
        if ((TargetLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("collision check");
            parentGoliath.PerformGrapple(hitGameObject, col.GetContact(0).point);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        
        if (!enabled)
        {
            return;
        }

        GameObject hitGameObject = col.gameObject;
        if ((TargetLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("trigger check");
            Vector2 contactPoint = col.GetComponent<Rigidbody2D>().ClosestPoint(transform.position);
            parentGoliath.PerformGrapple(hitGameObject, contactPoint);
        }
    }
}
