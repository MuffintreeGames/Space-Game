using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SendToFinalFight : MonoBehaviour
{
    private static GameObject goliath;
    // Start is called before the first frame update
    void Start()
    {
        if (goliath == null)
        {
            goliath = GameObject.Find("Goliath");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void GoToFinalFight()
    {
        goliath.GetComponent<GoliathController>().ResetState();
        if (PhotonNetwork.IsConnected) PhotonNetwork.LoadLevel("FinalFight");
        else SceneManager.LoadSceneAsync(4);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == goliath)
        {
            GoToFinalFight();
        }
    }
}
