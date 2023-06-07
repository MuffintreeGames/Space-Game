using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimeManager : MonoBehaviour
{
    private static float timeElapsed = 0f;
    private static bool timeRunning = false;    //used to pause clock before game starts + if something needs to pause the game

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            timeRunning = true;
            GameObject.Find("LoadingOverlay").SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRunning)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public static float GetElapsedTime()
    {
        return timeElapsed;
    }

    public static bool GetTimeRunning()
    {
        return timeRunning;
    }

    public static void SetTimeRunning(bool value)
    {
        timeRunning = value;
        if (timeRunning)
        {
            Time.timeScale = 1f;
        } else
        {
            Time.timeScale = 0f;
        }
    }
}
