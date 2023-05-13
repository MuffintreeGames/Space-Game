using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Online
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        [Tooltip("The Ui Panel to let the user start game")]
        [SerializeField]
        public static GameManager Instance;
        private GameObject controlPanel;
        #endregion


        #region Photon Callbacks
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.LogFormat("OnLeftRoom");
            SceneManager.LoadSceneAsync(2);
        }

        #endregion

        #region Public Methods

        void Start()
        {
            Instance = this;
        }

        public void LeaveRoom()
        {
            Debug.LogFormat("LeaveRoom"); // click Back to Online
            PhotonNetwork.LeaveRoom();
        }

        public void StartGame()
        {

            PhotonNetwork.LoadLevel("MainGame");
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            controlPanel.SetActive(false);
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork : Loading Level w/ Player Count : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            // We use PhotonNetwork.LoadLevel() to load the level we want, we don't use Unity directly.
            // Rely on Photon to load levels on all connected clients in the room, since we've enabled PhotonNetwork.AutomaticallySyncScene
            PhotonNetwork.LoadLevel("WaitingRoom");
        }


        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2) // change this to == 2 after testing
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    controlPanel.SetActive(true);
                }
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
                controlPanel.SetActive(false);
                LoadArena();
            }
        }

        #endregion
    }
}