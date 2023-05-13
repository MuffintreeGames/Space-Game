using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnClick : MonoBehaviourPun    //attach to object to have image follow mouse, and on a click place an object into the world
{
    public GameObject CreatedObject;
    public string helpText = "Left-click on empty area to place object. Right-click to cancel.";
    public bool canPlaceOnSolids = false;
    public bool spendOnSpawn = false;   //if true, triggers parent ability mana cost + cooldown
    public GodAbilityTemplate parentAbility;

    private GodController god;
    private LayerMask preventedLayers;
    private bool canPlace = true;
    private CircleCollider2D hoverCircleCollider;
    private BoxCollider2D hoverBoxCollider;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("GodController").GetComponent<GodController>();
        hoverCircleCollider = GetComponent<CircleCollider2D>();
        if (hoverCircleCollider == null)
        {
            hoverBoxCollider = GetComponent<BoxCollider2D>();
        }
        preventedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize1"));
        preventedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize2"));
        preventedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize3"));
        preventedLayers |= (1 << LayerMask.NameToLayer("DestructibleSize4"));
        preventedLayers |= (1 << LayerMask.NameToLayer("Solid"));
        preventedLayers |= (1 << LayerMask.NameToLayer("Goliath"));
        preventedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel1"));
        preventedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel2"));
        preventedLayers |= (1 << LayerMask.NameToLayer("BarrierLevel3"));
        preventedLayers |= (1 << LayerMask.NameToLayer("GoliathProjectile"));
        preventedLayers |= (1 << LayerMask.NameToLayer("GoliathDestructible"));
        preventedLayers |= (1 << LayerMask.NameToLayer("Indestructible"));
        preventedLayers |= (1 << LayerMask.NameToLayer("GodProjectile"));
        HUDManager.UpdateGodAbilityHelpText.Invoke(helpText);
    }

    // Update is called once per frame
    void Update()
    {
        god.SetAbilityUsage(false);
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseCoords;

        if (!canPlaceOnSolids)
        {
            Collider2D preventativeObject;
            if (hoverCircleCollider != null)
            {
                preventativeObject = Physics2D.OverlapCircle(mouseCoords, hoverCircleCollider.radius * transform.localScale.y, preventedLayers);
            }
            else
            {
                preventativeObject = Physics2D.OverlapBox(mouseCoords, hoverBoxCollider.size * transform.localScale.y, preventedLayers);
            }

            if (preventativeObject != null)
            {
                Debug.Log("found something in the way: " + preventativeObject.gameObject.name + " on layer " + LayerMask.LayerToName(preventativeObject.gameObject.layer));
                canPlace = false;
            }
            else
            {
                canPlace = true;
            }
        } else
        {
            canPlace = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                if (spendOnSpawn)
                {
                    if (!god.SpendMP(parentAbility.manaCost))
                    {
                        Debug.Log("Can't afford to place!");
                        return;
                    }
                    parentAbility.StartCooldown();
                }

                PhotonNetwork.Instantiate(CreatedObject.ToString(), mouseCoords, Quaternion.identity);
                god.SetAbilityUsage(true);
                HUDManager.UpdateGodAbilityHelpText.Invoke("");
                Destroy(gameObject);
            } else
            {
                Debug.Log("obstruction in way!");
            }
        } else if (Input.GetMouseButtonUp(1))
        {
            god.SetAbilityUsage(true);
            HUDManager.UpdateGodAbilityHelpText.Invoke("");
            Destroy(gameObject);
        }
    }
}
