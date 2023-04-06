using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnClick : MonoBehaviour    //attach to object to have image follow mouse, and on a click place an object into the world
{
    public GameObject CreatedObject;
    public float Cost = 25f;

    private GodController god;
    private LayerMask preventedLayers;
    private bool canPlace = true;
    private CircleCollider2D hoverCollider;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("GodController").GetComponent<GodController>();
        hoverCollider = GetComponent<CircleCollider2D>();
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
    }

    // Update is called once per frame
    void Update()
    {
        god.SetAbilityUsage(false);
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseCoords;

        Collider2D preventativeObject = Physics2D.OverlapCircle(mouseCoords, hoverCollider.radius * transform.localScale.y, preventedLayers);
        if (preventativeObject != null)
        {
            Debug.Log("found something in the way: " + preventativeObject.gameObject.name);
            canPlace = false;
        } else
        {
            canPlace = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                if (!god.SpendMP(Cost))
                {
                    Debug.Log("Can't afford to place!");
                    return;
                }
                Instantiate(CreatedObject, mouseCoords, Quaternion.identity);
                god.SetAbilityUsage(true);
                Destroy(gameObject);
            } else
            {
                Debug.Log("obstruction in way!");
            }
        } else if (Input.GetMouseButtonUp(1))
        {
            god.SetAbilityUsage(true);
            Destroy(gameObject);
        }
    }
}
