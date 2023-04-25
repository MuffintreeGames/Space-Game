using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour    //attach to any unique objects that should be maintained between scenes
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}