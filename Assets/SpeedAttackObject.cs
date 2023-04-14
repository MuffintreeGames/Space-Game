using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedAttackObject : AttackObject   //variant of attack object that does damage relative to current speed
{
    public float DamagePerSpeed = 1f;

    private Rigidbody2D rb;
    private Vector3 positionLastFrame;
    private bool initializing = true;
    private bool positionSet = false;

    private SlowableObject slowComponent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null )
        {
            Debug.LogError("Speed attack object without a rigidbody!");
        }
        slowComponent = rb.GetComponent<SlowableObject>();
        if (slowComponent == null)
        {
            Debug.LogError("Speed attack object without a slowable component!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (positionSet)    //have to do this first so that planet knows where it is
        {
            float xDifference = rb.position.x - positionLastFrame.x;
            float yDifference = rb.position.y - positionLastFrame.y;
            float currentSpeed = Mathf.Sqrt(xDifference * xDifference + yDifference * yDifference);
            if (initializing && currentSpeed != 0f)
            {
                initializing = false;
            }

            Damage = Mathf.RoundToInt(currentSpeed * DamagePerSpeed * slowComponent.GetSlowFactor());  //multiply by slow factor here to keep relative damage the same even when slowed
            if (Damage < 5)
            {
                Damage = 0;
                if (!initializing)
                {
                    Debug.Log("disabling speed attack");
                    initializing = true;
                    positionSet = false;
                    enabled = false;    //disable any that aren't moving to avoid excessive calculations
                }
            }
        }
        positionLastFrame = rb.position;
        positionSet = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (enabled)
        {
            SpeedAttackObject collidedAttackObject = collision.gameObject.GetComponent<SpeedAttackObject>();
            if (collidedAttackObject)
            {
                collidedAttackObject.enabled = true;
            }
            base.OnCollisionStay2D(collision);
        }
    }
}
