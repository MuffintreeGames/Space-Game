using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterHitChecker : MonoBehaviour  //attach to attack object to make it so that only the master client has the collider active
{

    public bool DisableAttack = false;  //if true, then instead of disabling collider, will only disable attack script. Useful if there's other effects to the attack
    private void Awake()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            if (!DisableAttack)
            {
                CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
                if (circleCollider != null)
                {
                    circleCollider.enabled = false;
                    return;
                }

                BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                    return;
                }
            } else
            {
                AttackObject attackObject = GetComponent<AttackObject>();
                if (attackObject != null)
                {
                    attackObject.enabled = false;
                }
            }
        }
    }
}
