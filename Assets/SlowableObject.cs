using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowableObject : MonoBehaviour
{
    private bool slowed;
    private float slowFactor = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSlowed(bool slowed)
    {
        this.slowed = slowed;
    }

    public float GetSlowFactor()
    {
        if (slowed)
        {
            return slowFactor;
        } else
        {
            return 1f;
        }
    }

    
}
