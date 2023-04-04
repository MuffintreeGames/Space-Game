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
            Debug.Log("Damage: " + Damage);
            if (Damage < 10)
            {
                Damage = 0;
            }
        }
        positionLastFrame = transform.position;
    }
}
