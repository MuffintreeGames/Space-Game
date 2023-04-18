using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExpGainEvent: UnityEvent<int>
{

}

public class EXPSource : MonoBehaviour
{
    //public delegate void PlanetKilled(int size);
    //public static event PlanetKilled GoliathPlanetKilled;

    public static ExpGainEvent GoliathGainExp;

    public int ExpAmount; //experience gained on a kill

    // Start is called before the first frame update
    void Awake()
    {
        if (GoliathGainExp == null)
        {
            GoliathGainExp = new ExpGainEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KilledByGoliath()
    {
        GoliathGainExp.Invoke(ExpAmount);
    }
}
