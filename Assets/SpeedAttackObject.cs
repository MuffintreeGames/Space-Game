using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAttackObject : AttackObject   //variant of attack object that does damage relative to current speed
{
    public float DamagePerSpeed = 1f;

    private Rigidbody2D rb;
    private Vector3 positionLastFrame;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null )
        {
            Debug.LogError("Speed attack object without a rigidbody!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (positionLastFrame != null)
        {
            float xDifference = transform.position.x - positionLastFrame.x;
            float yDifference = transform.position.y - positionLastFrame.y;
            float currentSpeed = Mathf.Sqrt(xDifference * xDifference + yDifference * yDifference);
            Damage = Mathf.RoundToInt(currentSpeed * DamagePerSpeed);
            if (Damage < 10)
            {
                Damage = 0;
            } else
            {
                Debug.Log("at damaging speeds!");
            }
            if (currentSpeed == 0)
            {
                enabled = false;    //disable any that aren't moving to avoid excessive calculations
            }
        }
        positionLastFrame = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpeedAttackObject collidedAttackObject = collision.gameObject.GetComponent<SpeedAttackObject>();
        if (collidedAttackObject)
        {
            collidedAttackObject.enabled = true;
            Debug.Log("billiards!");
        }
        base.OnCollisionEnter2D(collision);
    }
}
