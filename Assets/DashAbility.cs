using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : AbilityTemplate  //dash a short distance, maintaining some speed afterwards
{
    public float dashSpeed = 25f;   //speed that goliath moves during dash
    public float dashDuration = 0.4f;   //length of dash in seconds
    public float exitSpeed = 6f;    //speed of goliath after dash

    protected float currentDuration = 0f;
    protected bool currentlyDashing = false;

    protected float dashDirectionX = 0f;
    protected float dashDirectionY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        base.InitializeAbility(AbilityCategory.Dash);
    }

    // Update is called once per frame
    void Update()
    {
        base.ManageCooldown();
        if (currentlyDashing)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= dashDuration)
            {
                Debug.Log("ending dash");
                CancelAbility();
            }
        }
    }

    public override void UseNormalAbility()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        parentGoliath.LockMovement();   //need to prevent goliath from doing manual movement
        dashDirectionX = Input.GetAxisRaw("Horizontal");
        dashDirectionY = Input.GetAxisRaw("Vertical");

        float horizontalSpeed = 0f;
        float verticalSpeed = 0f;

        float adjustedDashSpeed = dashSpeed * parentGoliath.GetSizeSpeedMultiplier();
        if (dashDirectionX != 0 && dashDirectionY != 0) //reduce dash speed when going diagonally
        {
            adjustedDashSpeed = Mathf.Sqrt((adjustedDashSpeed * adjustedDashSpeed)/2);
        }

        if (dashDirectionX > 0)
        {
            horizontalSpeed = adjustedDashSpeed;
        } else if (dashDirectionX < 0)
        {
            horizontalSpeed = -adjustedDashSpeed;
        }

        if (dashDirectionY > 0)
        {
            verticalSpeed = adjustedDashSpeed;
        }
        else if (dashDirectionY < 0)
        {
            verticalSpeed = -adjustedDashSpeed;
        }

        parentGoliath.SetSpeedExternally(horizontalSpeed, verticalSpeed);
        currentDuration = 0f;
        currentlyDashing = true;
    }

    public override void CancelAbility()
    {
        PrepareToEndAbility();
        currentlyDashing = false;
        parentGoliath.UnlockMovement();
        float exitX = 0f;
        float exitY = 0f;

        if (dashDirectionX > 0)
        {
            exitX = exitSpeed;
        } else if (dashDirectionX < 0)
        {
            exitX = -exitSpeed;
        }

        if (dashDirectionY > 0)
        {
            exitY = exitSpeed;
        }
        else if (dashDirectionY < 0)
        {
            exitY = -exitSpeed;
        }

        parentGoliath.SetSpeedExternally(exitX, exitY);
    }
}
