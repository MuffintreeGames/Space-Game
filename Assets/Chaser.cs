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

    // Start is called before the first frame update
    void Start()
    {
        goliathRigid = GameObject.Find("Goliath").GetComponent<Rigidbody2D>();
        chaserRigid = GetComponent<Rigidbody2D>();
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
        Vector2 oldVelocity = chaserRigid.velocity;
        Vector2 currentVelocity = chaserRigid.velocity;
        Vector2 changeInVelocity = new Vector2(0, 0);
        /*if (targetDirection.x > 0 && currentVelocity.x + (targetDirection.x * acceleration) > maxSpeed)
        {
            changeInVelocity.x = Mathf.Max(0, maxSpeed - )
        }*/

        //if (!(currentVelocity.x > maxSpeed && targetDirection.x > 0) && !((currentVelocity.x < -maxSpeed && targetDirection.x < 0)))
        //{
        changeInVelocity.x = targetDirection.normalized.x * acceleration * chaserRigid.mass;
        //}

        //if (!(currentVelocity.y > maxSpeed && targetDirection.y > 0) && !((currentVelocity.y < -maxSpeed && targetDirection.y < 0)))
        //{
        changeInVelocity.y = targetDirection.normalized.y * acceleration * chaserRigid.mass;
        //}

        chaserRigid.AddForce(changeInVelocity);
        Vector2 adjustedVelocity = chaserRigid.velocity;
        //Debug.Log("velocity = " + adjustedVelocity);
        if (/*oldVelocity.x <= maxSpeed &&*/ chaserRigid.velocity.x > maxSpeed)
        {
            Debug.Log("adjusting x");
            adjustedVelocity.x = maxSpeed;
        }
        else if (/*oldVelocity.x >= -maxSpeed &&*/ chaserRigid.velocity.x < -maxSpeed)
        {
            Debug.Log("adjusting x");
            adjustedVelocity.x = -maxSpeed;
        }

        if (/*oldVelocity.y <= maxSpeed &&*/ chaserRigid.velocity.y > maxSpeed)
        {
            Debug.Log("adjusting y");
            adjustedVelocity.y = maxSpeed;
        }
        else if (/*oldVelocity.y >= -maxSpeed &&*/ chaserRigid.velocity.y < -maxSpeed)
        {
            Debug.Log("adjusting y");
            adjustedVelocity.y = -maxSpeed;
        }

        chaserRigid.velocity = adjustedVelocity;
    }
}
