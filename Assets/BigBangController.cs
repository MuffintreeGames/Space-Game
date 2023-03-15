using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBangController : MonoBehaviour
{
    public float GrowthRate; //how fast should big bang expand

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1,1,0) * GrowthRate * Time.deltaTime;
    }
}
