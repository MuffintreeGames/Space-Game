using Online;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleDisconnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogError("opponent disconnected!");
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 1 || sceneIndex == 4)    //not in game anymore. Update this if any extra scenes get added to the main game
        {
            GameManager.Instance.LeaveRoom();
            SceneManager.LoadScene("DisconnectedScreen");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.LogError("disconnected!");
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 1 || sceneIndex == 4)    //not in game anymore. Update this if any extra scenes get added to the main game
        {
            GameManager.Instance.LeaveRoom();
            SceneManager.LoadScene("SelfDisconnectScreen");
        }
    }
}
