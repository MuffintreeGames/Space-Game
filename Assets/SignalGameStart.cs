using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalGameStart : MonoBehaviour    //when the object with this script attached has been spawned for the non-master client, that indicates that the game is ready to begin
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView.Get(this).RPC("RpcStartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void RpcStartGame()
    {
        Debug.Log("starting game!");
        GameObject.Find("LoadingOverlay").SetActive(false);
        TimeManager.SetTimeRunning(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
