using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityGainEvent : UnityEvent<AbilityTemplate>
{

}

public class GrantAbility : MonoBehaviour, IPunInstantiateMagicCallback
{
    public static AbilityGainEvent GoliathGainAbility;

    public AbilityTemplate GrantedAbility;

    private Image childSprite;
    private bool spriteSet = false;

    object[] instantiationData;

    // Start is called before the first frame update
    private void Awake()
    {
        if (GoliathGainAbility == null)
        {
            GoliathGainAbility = new AbilityGainEvent();
        }
    }
    void Start()
    {
        GameObject childImage = transform.GetChild(0).gameObject;
        if (childImage)
        {
            childSprite = childImage.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (childSprite && GrantedAbility != null && !spriteSet)
        {
            childSprite.sprite = GrantedAbility.icon;
            spriteSet = true;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        instantiationData = info.photonView.InstantiationData;
        if (SpaceManager.WorldMap != null)
        {
            SetAbility();
        } else
        {
            Debug.Log("ability planet spawned before world map available!");
        }
    }

    void SetAbility()
    {
        int abilityIndex = (int)instantiationData[0];
        Debug.Log("looking for ability " + abilityIndex);
        GrantedAbility = GameObject.Find("SpaceManager").GetComponent<SpaceManager>().AbilityPool[abilityIndex];
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int sectorX = (int)instantiationData[1];
        int sectorY = (int)instantiationData[2];
        int chunkX = (int)instantiationData[3];
        int chunkY = (int)instantiationData[4];
        //Debug.Log("registering planet: " + (int)instantiationData[0] + "," + (int)instantiationData[1] + "," + (int)instantiationData[2] + "," + (int)instantiationData[3]);
        SpaceManager.WorldMap[sectorX][sectorY][chunkX][chunkY] = gameObject;
    }

    public void KilledByGoliath()
    {
        GoliathGainAbility.Invoke(GrantedAbility);
    }
}
