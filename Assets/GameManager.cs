using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Online
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        [Tooltip("The Ui Panel to let the user start game")]
        [SerializeField]
        public static GameManager Instance;
        public GameObject controlPanel;
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

        public void StartGameGoliath()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            SetCustomPlayerProperties(PhotonNetwork.IsMasterClient);
        }

        public void StartGameGod()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            SetCustomPlayerProperties(!PhotonNetwork.IsMasterClient);
        }

        bool ready = false;
        bool opponentReady = false;
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer != targetPlayer)
            {
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                if (changedProps.ContainsKey("isGoliath"))
                {
                    Hashtable roomProperties = new Hashtable();
                    roomProperties.Add("opponentReady", true);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                }
                return;
            }

            if (!ready) {
                if (changedProps.ContainsKey("isGoliath"))
                {
                    ready = true;
                }
            }

            if (ready && opponentReady)
            {
                PhotonNetwork.LoadLevel("MainGame");
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!opponentReady)
            {
                if (propertiesThatChanged.ContainsKey("opponentReady"))
                {
                    opponentReady = true;
                }
            }

            if (ready && opponentReady)
            {
                PhotonNetwork.LoadLevel("MainGame");
            }
        }

        public void SetCustomPlayerProperties(bool isGoliath)
        {
            Hashtable localProperties = new Hashtable();
            int seed = Random.Range(1, int.MaxValue);
            localProperties.Add("Seed", seed);
            localProperties.Add("isGoliath", isGoliath);
            PhotonNetwork.LocalPlayer.SetCustomProperties(localProperties);

            Hashtable opponentProperties = new Hashtable();
            opponentProperties.Add("Seed", seed);
            opponentProperties.Add("isGoliath", !isGoliath);
            PhotonNetwork.PlayerList[1].SetCustomProperties(opponentProperties);
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            controlPanel.SetActive(true);
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