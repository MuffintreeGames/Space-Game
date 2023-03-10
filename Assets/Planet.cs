using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Planet : MonoBehaviour
{
    //public delegate void PlanetKilled(int size);
    //public static event PlanetKilled GoliathPlanetKilled;

    public static UnityEvent GoliathSmallPlanetKilled;
    public static UnityEvent GoliathMediumPlanetKilled;
    public static UnityEvent GoliathLargePlanetKilled;
    public static UnityEvent GoliathMassivePlanetKilled;

    public int size; //size of this planet

    // Start is called before the first frame update
    void Start()
    {
        if (GoliathSmallPlanetKilled == null)
        {
            GoliathSmallPlanetKilled = new UnityEvent();
        }
        if (GoliathMediumPlanetKilled == null)
        {
            GoliathMediumPlanetKilled = new UnityEvent();
        }
        if (GoliathLargePlanetKilled == null)
        {
            GoliathLargePlanetKilled = new UnityEvent();
        }
        if (GoliathMassivePlanetKilled == null)
        {
            GoliathMassivePlanetKilled = new UnityEvent();
        }
        if (size < 1 || size > 4)
        {
            size = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        switch (size)
        {
            case 1:
                GoliathSmallPlanetKilled.Invoke();
                break;
            case 2:
                GoliathMediumPlanetKilled.Invoke();
                break;
            case 3:
                GoliathLargePlanetKilled.Invoke();
                break;
            case 4:
                GoliathMassivePlanetKilled.Invoke();
                break;
        }
    }
}
