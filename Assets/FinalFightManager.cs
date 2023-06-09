using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalFightManager : MonoBehaviour  //use to do any things that should happen at the start of the final fight
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Goliath").transform.position = Vector3.zero;
        GameObject.Find("GodAvatar").transform.position = new Vector3(0, 15, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
