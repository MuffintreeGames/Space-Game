using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum directionChoices
    {
        right,
        up
    };

    public float Speed; //speed of projectile
    public float LifeTime;  //max time that projectile can be active
    public directionChoices direction = directionChoices.right; //whether this projectile should fly right or up relative to its parent. Use whichever one gives correct behaviour pretty much

    private float angle = 90f; //angle of projectile
    private float currentTime = 0f; //length of time that projectile has been active

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= LifeTime)
        {
            Destroy(this.gameObject);
        }

        if (direction == directionChoices.right)
        {
            transform.position += transform.right * Speed * Time.deltaTime;
        } else
        {
            transform.position += transform.up * Speed * Time.deltaTime;
        }

        currentTime += Time.deltaTime;
    }

    public void SetProjectileParameters(float targetSpeed, float targetAngle, float targetTime)
    {
        Speed = targetSpeed;
        angle = targetAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        LifeTime = targetTime;
    }
}