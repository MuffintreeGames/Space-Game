using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetScript : MonoBehaviour, IPunInstantiateMagicCallback //simple script to disable object immediately on spawn
{
    object[] instantiationData = null;
    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        instantiationData = info.photonView.InstantiationData;
        if (SpaceManager.WorldMap != null)
        {
            RegisterCoordinates();
        }
    }

    void RegisterCoordinates()
    {
        int sectorX = (int)instantiationData[0];
        int sectorY = (int)instantiationData[1];
        int chunkX = (int)instantiationData[2];
        int chunkY = (int)instantiationData[3];
        Debug.Log("registering planet: " + (int)instantiationData[0] + "," + (int)instantiationData[1] + "," + (int)instantiationData[2] + "," + (int)instantiationData[3]);
        SpaceManager.WorldMap[sectorX][sectorY][chunkX][chunkY] = gameObject;

        if (SpaceManager.CoordsInStartingSector(sectorX, sectorY))
        {
            gameObject.SetActive(true);
        }
    }
}
