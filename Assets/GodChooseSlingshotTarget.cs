using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodChooseSlingshotTarget : MonoBehaviourPun    //select a planet to launch
{
    public GameObject AimingArrow;
    public string helpText;

    private GodController god;
    private LayerMask validLayers;
    private bool canPlace = false;
    public GodAbilityTemplate parentAbility;
    private CircleCollider2D hoverCollider;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("GodController").GetComponent<GodController>();
        hoverCollider = GetComponent<CircleCollider2D>();
        validLayers |= (1 << LayerMask.NameToLayer("DestructibleSize1"));
        validLayers |= (1 << LayerMask.NameToLayer("DestructibleSize2"));
        validLayers |= (1 << LayerMask.NameToLayer("DestructibleSize3"));
        validLayers |= (1 << LayerMask.NameToLayer("DestructibleSize4"));
        HUDManager.UpdateGodAbilityHelpText.Invoke(helpText);
    }

    // Update is called once per frame
    void Update()
    {
        god.SetAbilityUsage(false);
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseCoords;

        Collider2D validObject = Physics2D.OverlapCircle(mouseCoords, hoverCollider.radius * transform.localScale.y, validLayers);
        if (validObject != null)
        {
            canPlace = true;
        }
        else
        {
            canPlace = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                GameObject newArrow;
                if (PhotonNetwork.IsConnected) newArrow = PhotonNetwork.Instantiate(AimingArrow.name, mouseCoords, Quaternion.identity);
                else newArrow = Instantiate(AimingArrow, mouseCoords, Quaternion.identity);
                AimingArrow arrowScript = newArrow.GetComponent<AimingArrow>();
                arrowScript.aimedObject = (CircleCollider2D) validObject;
                arrowScript.parentAbility = parentAbility;
                HUDManager.UpdateGodAbilityHelpText.Invoke("");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("nothing there!");
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            god.SetAbilityUsage(true);
            HUDManager.UpdateGodAbilityHelpText.Invoke("");
            Destroy(gameObject);
        }
    }
}
