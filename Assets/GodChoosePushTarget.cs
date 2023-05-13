using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodChoosePushTarget : MonoBehaviourPun   //select where to create pushing wall
{
    public GameObject AimingArrow;
    public string helpText;

    private GodController god;
    private bool canPlace = true;
    public GodAbilityTemplate parentAbility;
    private CircleCollider2D hoverCollider;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("GodController").GetComponent<GodController>();
        hoverCollider = GetComponent<CircleCollider2D>();
        HUDManager.UpdateGodAbilityHelpText.Invoke(helpText);
    }

    // Update is called once per frame
    void Update()
    {
        god.SetAbilityUsage(false);
        Vector2 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseCoords;

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                GameObject newArrow = PhotonNetwork.Instantiate(AimingArrow.ToString(), mouseCoords, Quaternion.identity);
                AimingArrow arrowScript = newArrow.GetComponent<AimingArrow>();
                arrowScript.parentAbility = parentAbility;
                HUDManager.UpdateGodAbilityHelpText.Invoke("");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("obstruction in way!");
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
