using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour //chases after the goliath
{
    public float maxSpeed = 3f;
    public float acceleration = 2f;
    private Rigidbody2D goliathRigid;
    private Rigidbody2D chaserRigid;

    private SlowableObject slowComponent;

    // Start is called before the first frame update
    void Start()
    {
        goliathRigid = GameObject.Find("Goliath").GetComponent<Rigidbody2D>();
        chaserRigid = GetComponent<Rigidbody2D>();
        slowComponent = GetComponent<SlowableObject>();
    }

    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 targetDirection = goliathRigid.position - chaserRigid.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        chaserRigid.SetRotation(angle - 90f);
        Vector2 changeInVelocity = new Vector2(0, 0);

        float currentMaxSpeed = maxSpeed / slowComponent.GetSlowFactor();
        float currentAcceleration = acceleration / slowComponent.GetSlowFactor();

        changeInVelocity.x = targetDirection.normalized.x * currentAcceleration * chaserRigid.mass;
        changeInVelocity.y = targetDirection.normalized.y * currentAcceleration * chaserRigid.mass;

        chaserRigid.AddForce(changeInVelocity);


        Vector2 adjustedVelocity = chaserRigid.velocity;
        if (/*oldVelocity.x <= maxSpeed &&*/ chaserRigid.velocity.x > currentMaxSpeed)
        {
            Debug.Log("adjusting x");
            adjustedVelocity.x = currentMaxSpeed;
        }
        else if (/*oldVelocity.x >= -maxSpeed &&*/ chaserRigid.velocity.x < -currentMaxSpeed)
        {
            Debug.Log("adjusting x");
            adjustedVelocity.x = -currentMaxSpeed;
        }

        if (/*oldVelocity.y <= maxSpeed &&*/ chaserRigid.velocity.y > currentMaxSpeed)
        {
            Debug.Log("adjusting y");
            adjustedVelocity.y = currentMaxSpeed;
        }
        else if (/*oldVelocity.y >= -maxSpeed &&*/ chaserRigid.velocity.y < -currentMaxSpeed)
        {
            Debug.Log("adjusting y");
            adjustedVelocity.y = -currentMaxSpeed;
        }

        chaserRigid.velocity = adjustedVelocity;
    }
}
