using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour    //attach to any unique game objects that should be maintained between scenes. Auto-deletes the objects when the game is left
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

    private void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != 1 && sceneIndex != 4)    //not in game anymore. Update this if any extra scenes get added to the main game
        {
            Destroy(this.gameObject);
        }
    }
}