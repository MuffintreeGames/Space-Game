using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Online
{
    public class Online : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [Tooltip("The UI Panel to navigate submenus")]
        [SerializeField]
        private GameObject topPanel;
        # endregion

        #region Private Serializable Fields
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;
        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            topPanel.SetActive(true);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            Debug.Log("Connect()");
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            topPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void Menu()
        {
            SceneManager.LoadSceneAsync(0);
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                Debug.Log("JoinRandRoom'");
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            topPanel.SetActive(true);
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom, PublishUserId = true});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
            Debug.Log("We load the 'WaitingRoom'");
            // #Critical: Load the Room Level.
            PhotonNetwork.LoadLevel("WaitingRoom");
        }

        #endregion

    }
}