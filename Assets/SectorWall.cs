using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SectorUnlockedEvent: UnityEvent<int, int>
{

}

public class SectorWall : MonoBehaviour
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
