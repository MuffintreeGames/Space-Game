using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimingArrow : MonoBehaviourPun
{
    private GodController god;
    //private Vector2 startingPos;    //position of arrow at creation
    public float launchForce = 15f;   //force applied to object
    public bool shootingObject = false; //if true, this should be attached to a specific game object. If false, should be anchored in space instead
    public CircleCollider2D aimedObject = null;
    public GameObject projectile;   //object to be spawned if not launching an existing object
    public GodAbilityTemplate parentAbility;
    public string helpText; //text displayed while aiming

    private bool appliedScale = false;
    private GameObject placementBlocker;
    private LayerMask invalidLayers;

    // Start is called before the first frame update
    void Start()
    {
        //startingPos = transform.position;
        god = GameObject.Find("GodController").GetComponent<GodController>();
        HUDManager.UpdateGodAbilityHelpText.Invoke(helpText);
        if (shootingObject)
        {
            placementBlocker = GameObject.Find("NoPlacementZone");
            placementBlocker.GetComponent<SpriteRenderer>().enabled = true;
        }
        invalidLayers |= (1 << LayerMask.NameToLayer("BlockPlacement"));
    }

    // Update is called once per frame
    void Update()
    {
        if (shootingObject)
        {
            if (aimedObject == null)
            {
                Destroy(gameObject);
            }

            Collider2D invalidObject = Physics2D.OverlapCircle(aimedObject.transform.position, aimedObject.transform.localScale.y, invalidLayers);  //check if object is in no placement zone
            if (invalidObject != null)
            {
                Destroy(gameObject);
            }


            if (!appliedScale)
            {
                appliedScale = true;
                transform.localScale = new Vector2(transform.localScale.x * aimedObject.transform.localScale.x, transform.localScale.y * aimedObject.transform.localScale.y) / 1.5f;
            }
        }

        Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetDirection = Vector3.zero;
        if (shootingObject)
        {
            if (aimedObject != null)
            {
                targetDirection = mouseCoords - aimedObject.transform.position;
            }
        } else
        {
            targetDirection = mouseCoords - transform.position;
        }

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (shootingObject)
        {
            float distanceFromCenter = aimedObject.radius * aimedObject.transform.localScale.x;
            transform.position = aimedObject.transform.position + (targetDirection.normalized * distanceFromCenter);
        }
        transform.localRotation = targetRotation;

        if (Input.GetMouseButtonDown(0))
        {
            god.SpendMP(parentAbility.manaCost);
            parentAbility.StartCooldown();
            if (shootingObject)
            {
                Rigidbody2D aimedRigid = aimedObject.gameObject.GetComponent<Rigidbody2D>();
                SlowableObject slowComponent = aimedObject.gameObject.GetComponent<SlowableObject>();   //apply slowdown if relevant
                aimedRigid.AddForce(targetDirection.normalized * launchForce * aimedRigid.mass / slowComponent.GetSlowFactor(), ForceMode2D.Impulse);
                if (PhotonNetwork.IsConnected) {
                    PhotonView aimedView = aimedObject.GetComponent<PhotonView>();
                    //aimedView.RequestOwnership();
                    aimedView.RPC("LaunchSpeedAttackObject", RpcTarget.All, (Vector2)targetDirection.normalized * launchForce * aimedRigid.mass / slowComponent.GetSlowFactor());
                }
                else
                {

                    SpeedAttackObject aimedAttack = aimedObject.GetComponent<SpeedAttackObject>();
                    aimedAttack.enabled = true;
                }
            } else
            {
                if (PhotonNetwork.IsConnected)
                {
                    object[] instantiationData = new object[3];
                    instantiationData[0] = (float)-1;
                    instantiationData[1] = targetRotation.eulerAngles.z;
                    instantiationData[2] = (float)-1;
                    PhotonNetwork.Instantiate(projectile.name, transform.position, Quaternion.identity, 0, instantiationData);
                }
                else
                {
                    GameObject firedShot = Instantiate(projectile, transform.position, Quaternion.identity);
                    Projectile shotScript = firedShot.GetComponent<Projectile>();
                    shotScript.SetProjectileParameters(shotScript.Speed, targetRotation.eulerAngles.z, shotScript.LifeTime);
                }
            }
            
            //god.SetAbilityUsage(true);
            //HUDManager.UpdateGodAbilityHelpText.Invoke("");
            Destroy(gameObject);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            //god.SetAbilityUsage(true);
            //HUDManager.UpdateGodAbilityHelpText.Invoke("");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        god.SetAbilityUsage(true);
        HUDManager.UpdateGodAbilityHelpText.Invoke("");
        if (placementBlocker != null)
        {
            placementBlocker.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
