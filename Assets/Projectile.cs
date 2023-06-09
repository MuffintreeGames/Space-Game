using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPunInstantiateMagicCallback
{
    public enum directionChoices
    {
        right,
        up
    };

    public float Speed; //speed of projectile
    public float LifeTime;  //max time that projectile can be active
    public directionChoices direction = directionChoices.right; //whether this projectile should fly right or up relative to its parent. Use whichever one gives correct behaviour pretty much

    public float Acceleration = 0f;     //change in travel speed over time
    public bool CanStop = false;        //if true, a projectile with negative acceleration can eventually stop

    private float angle = 90f; //angle of projectile
    private float currentTime = 0f; //length of time that projectile has been active

    private SlowableObject slowComponent;

    // Start is called before the first frame update
    void Start()
    {
        slowComponent = GetComponent<SlowableObject>();
        if (slowComponent == null)
        {
            Debug.LogError("Projectile object without a slowable component!");
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        if (instantiationData == null) {
            return;
        }

        if (instantiationData.Length == 5) {
            SetProjectileParameters((float)instantiationData[0], (float)instantiationData[1], (float)instantiationData[2], (float)instantiationData[3], (bool)instantiationData[4]);
        }
        else
        {
            SetProjectileParameters((float)instantiationData[0], (float)instantiationData[1], (float)instantiationData[2]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= LifeTime)
        {
            ProjectileExpire();
        }

        if (direction == directionChoices.right)
        {
            transform.position += transform.right * Speed * Time.deltaTime / slowComponent.GetSlowFactor();
        } else
        {
            transform.position += transform.up * Speed * Time.deltaTime / slowComponent.GetSlowFactor();
        }

        currentTime += Time.deltaTime / slowComponent.GetSlowFactor();    //projectile lasts longer if slowed

        Speed += Acceleration * Time.deltaTime;
        if (CanStop && Speed <= 0f)
        {
            Speed = 0f;
        }
    }

    protected virtual void ProjectileExpire()
    {
        Destroy(this.gameObject);
    }

    public void SetProjectileParameters(float targetSpeed, float targetAngle, float targetTime, float acceleration = 0f, bool canStop = false)
    {
        if (targetSpeed >= 0f)
        {
            Speed = targetSpeed;
        }

        angle = targetAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);

        if (targetTime >= 0f)
        {
            LifeTime = targetTime;
        }
        Acceleration = acceleration;
        CanStop = canStop;
    }
}
