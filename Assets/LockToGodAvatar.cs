using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToGodAvatar : MonoBehaviour    //attach to any attacks that are designed to be relative to the god avatar
{
    private Transform avatarTransform;
    // Start is called before the first frame update
    void Start()
    {
        avatarTransform = GameObject.Find("GodAvatar").transform;
        if (avatarTransform == null )
        {
            Debug.LogError("Couldn't find god avatar!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( avatarTransform != null )
        {
            transform.position = avatarTransform.position;
        }
    }
}
