using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalyzeOnTouchScript : MonoBehaviour  //paralyze the goliath on touch
{
    public float paralysisRate = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject triggerObject = collision.gameObject;
        if (triggerObject.name == "Goliath")
        {
            triggerObject.GetComponent<GoliathController>().ApplyParalysis(paralysisRate * Time.deltaTime);
        }
    }
}
