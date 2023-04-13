using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAwayScript : MonoBehaviour //paralyze the goliath if they look at this for too long
{
    public float paralysisRate = 0.3f;
    private GoliathController playerGoliath;

    // Start is called before the first frame update
    void Start()
    {
        playerGoliath = GameObject.Find("Goliath").GetComponent<GoliathController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject triggerObject = collision.gameObject;
        if (triggerObject.layer != LayerMask.NameToLayer("GoliathSight")) {
            return;
        }

        playerGoliath.ApplyParalysis(paralysisRate * Time.deltaTime);
    }
}
