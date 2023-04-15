using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Online
{
    public class WaitingRoom : MonoBehaviour
    {
        #region Public Fields
        [Tooltip("The UI Label to inform the user of current Match state")]
        [SerializeField]
        private TMP_Text stateLabel;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            string playerList = "";
            for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount + 1; i++)
            {
                playerList += PhotonNetwork.CurrentRoom.Players[i].ToStringFull() + System.Environment.NewLine;
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2) // change this to == 2 after testing
            {
                playerList += "Room Closed!";
            }
            stateLabel.SetText(playerList);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
