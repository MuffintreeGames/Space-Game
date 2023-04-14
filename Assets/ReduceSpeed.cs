using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceSpeed : MonoBehaviour
{
    public static float slowStrength = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*Projectile colProjectile = collision.GetComponent<Projectile>();
        if (colProjectile != null)
        {
            colProjectile.SetSlowed(true);
            return;
        }*/

        Rigidbody2D colRigid = collision.GetComponent<Rigidbody2D>();
        if (colRigid == null)
        {
            return;
        }
        colRigid.velocity = colRigid.velocity / slowStrength;

        /*GoliathController colGoliath = collision.GetComponent<GoliathController>();
        if (colGoliath != null)
        {
            colGoliath.SetSlowed(true);
            return;
        }

        Chaser colChaser = collision.GetComponent<Chaser>();
        if (colChaser != null)
        {
            colChaser.SetSlowed(true);
            return;
        }

        SpeedAttackObject colSpeedAttack = collision.GetComponent<SpeedAttackObject>();
        if (colSpeedAttack != null)
        {
            colSpeedAttack.SetSlowed(true);
        }*/

        SlowableObject colSlowable = collision.GetComponent<SlowableObject>();
        if (colSlowable != null)
        {
            colSlowable.SetSlowed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*Projectile colProjectile = collision.GetComponent<Projectile>();
        if (colProjectile != null)
        {
            colProjectile.SetSlowed(false);
            return;
        }*/

        Rigidbody2D colRigid = collision.GetComponent<Rigidbody2D>();
        if (colRigid == null)
        {
            return;
        }
        colRigid.velocity = colRigid.velocity * slowStrength;

        /*GoliathController colGoliath = collision.GetComponent<GoliathController>();
        if (colGoliath != null)
        {
            colGoliath.SetSlowed(false);
            return;
        }

        Chaser colChaser = collision.GetComponent<Chaser>();
        if (colChaser != null)
        {
            colChaser.SetSlowed(false);
            return;
        }

        SpeedAttackObject colSpeedAttack = collision.GetComponent<SpeedAttackObject>();
        if (colSpeedAttack != null)
        {
            colSpeedAttack.SetSlowed(false);
        }*/

        SlowableObject colSlowable = collision.GetComponent<SlowableObject>();
        if (colSlowable != null)
        {
            colSlowable.SetSlowed(false);
        }
    }
}
