using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SectorUnlockedEvent: UnityEvent<int, int>
{

}

public class SectorWall : MonoBehaviour, IPunInstantiateMagicCallback
{

    private int sectorX;
    private int sectorY;

    public static SectorUnlockedEvent UnlockSector;

    void Awake()
    {
        if (UnlockSector == null)
        {
            UnlockSector = new SectorUnlockedEvent();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        object[] instantiationData = info.photonView.InstantiationData;

        SetParameters((int)instantiationData[0], (int)instantiationData[1]);
    }

    public void SetParameters(int x, int y)
    {
        sectorX = x;
        sectorY = y;
    }

    void OnDestroy()
    {
        UnlockSector.Invoke(sectorX, sectorY);
    }
}
