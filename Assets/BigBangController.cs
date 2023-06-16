using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBangController : MonoBehaviour
{
    public float BaseGrowthRate; //how fast should big bang expand
    public float Acceleration;  //how quickly the growth rate increases

    private float GrowthRate;

    // Start is called before the first frame update
    void Start()
    {
        GrowthRate = BaseGrowthRate;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1,1,0) * GrowthRate * Time.deltaTime;
        GrowthRate += Acceleration * Time.deltaTime;
    }
}
