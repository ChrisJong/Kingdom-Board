namespace KingdomBoard.Manager {

	using UnityEngine;

	using Photon.Pun;
	using Photon.Realtime;

	using Extension;
	using ExitGames.Client.Photon;

	public enum NetworkState {
        NONE = 0,
        ANY = ~0,
		CONNECTING_SERVER = 1,
        IN_SERVER,
		CONNECTING_LOBBY,
		IN_LOBBY,
        CONNECTING_GAMEROOM,
		WAITING_GAMEROOM,
        IN_GAMEROOM,
        DISCONNECTED,
        OFFLINE,
    }

	public class NetworkManager : SingletonMonoPunCallbacks<NetworkManager> {

		#region VARIABLE
		[SerializeField] private bool _isConnectedToServer = false;
		[SerializeField] private bool _isMasterClient = false;

		[SerializeField] private string _lobbyName = string.Empty;
		[SerializeField] private string _roomName = string.Empty;

		[Space]
		[SerializeField] private NetworkState _currentState = NetworkState.NONE;
		[SerializeField] private NetworkState _previousState = NetworkState.NONE;

		[SerializeField] private LoadBalancingClient client = null;
		private TypedLobby _currentLobby = null;
		private RoomOptions _roomOptions = new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true };
		private Room _currentRoom = null;

		public bool IsConnectedToServer { get { return this._isConnectedToServer; } }
		public bool isMasterClient { get { return this._isMasterClient; } }

		public NetworkState CurrentState { get { return this._currentState; } }
		public NetworkState PreviousdState { get { return this._previousState; } }
		#endregion

		#region UNITY

		protected override void Awake() {
			base.Awake();

			DontDestroyOnLoad(this);

			// NOTE: need to change the networking using the loading balance for better debugging.
			//this.client = new LoadBalancingClient();
			//this.client.Connect();

			//this.ConnectToServer();
		}

		#endregion

		#region PHOTON_PUN
		public override void OnConnectedToMaster() {
			Debug.Log("Connected to the Master Server");
			this._isConnectedToServer = true;
			this.ChangeState(NetworkState.IN_SERVER);

			// Connect To the Master Lobby.
			MenuManager.instance.ConnectedToServer();
			this.ConnectToLobby();
		}

		public override void OnDisconnected(DisconnectCause cause) {
			Debug.LogWarning("Disconnected From the Master Server, Cause: " + cause);

			this._isConnectedToServer = false;
			this.ChangeState(NetworkState.DISCONNECTED);

			/*if(!this.RetryConnectionToServer())
				MenuManager.instance.DisonnectedToServer();*/
		}

		public override void OnJoinedLobby() {
			Debug.LogWarning("Connected To Master Lobby");

			this.ChangeState(NetworkState.IN_LOBBY);

			this._currentLobby = PhotonNetwork.CurrentLobby;
			this._lobbyName = this._currentLobby.Name;
		}

		public override void OnLeftLobby() {
			Debug.LogWarning("Left The Master Lobby");

			this.ChangeState(NetworkState.IN_SERVER);

			this._currentLobby = null;
			this._lobbyName = string.Empty;
		}

		public override void OnCreatedRoom() {
			Debug.LogWarning("Created Game Room, Finding Players!");

			this.ChangeState(NetworkState.WAITING_GAMEROOM);
			this._currentRoom = PhotonNetwork.CurrentRoom;
			this._roomName = this._currentRoom.Name;
		}

		public override void OnCreateRoomFailed(short returnCode, string message) {
			Debug.LogWarning("Creating Game Room Failed! cause of: " + message + " (" + returnCode.ToString() + ")");

			// Try to Join a random room if we failed. otherwise go into offline mode.
		}

		public override void OnPlayerEnteredRoom(Player newPlayer) {
			if(PhotonNetwork.IsMasterClient)
				Debug.LogWarning("Another Player has entered the game room, Starting Game!");

			this.ChangeState(NetworkState.IN_GAMEROOM);

			Debug.LogWarning("Number Of Players: " + this._currentRoom.PlayerCount.ToString());
			Debug.LogWarning("Player Name: " + newPlayer.NickName);
			Debug.LogWarning("Player Master Client? " + newPlayer.IsMasterClient.ToString());
			Debug.LogWarning("Player ACtor Name/ID: " + newPlayer.ActorNumber);

			MenuManager.instance.StartGame();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer) {
			if(PhotonNetwork.IsMasterClient)
				Debug.LogWarning("Another Player has left the game room: " + otherPlayer.ActorNumber.ToString());
		}

		public override void OnJoinedRoom() {
			Debug.LogWarning("Joined a random Game room!, Starting Game!");

			this.ChangeState(NetworkState.IN_GAMEROOM);

			this._currentRoom = PhotonNetwork.CurrentRoom;
			this._roomName = this._currentRoom.Name;

			if(this._currentRoom.PlayerCount == 2)
				MenuManager.instance.StartGame();
		}

		public override void OnJoinRoomFailed(short returnCode, string message) {
			Debug.LogWarning("Failed to join a Named Room: " + message + " (" + returnCode.ToString() + ")");
		}

		public override void OnJoinRandomFailed(short returnCode, string message) {
			Debug.LogWarning("Failed to randomly join a game room: " + message + " (" + returnCode.ToString() + ")");

			this.CreateGameRoom();
		}

		public override void OnLeftRoom() {
			Debug.LogWarning("Leaving Game Room / Cancel Search!");

			this._currentRoom = null;
			this._roomName = string.Empty;
		}

		public override void OnMasterClientSwitched(Player newMasterClient) {
			Debug.LogWarning("The Current Master Client has Disconnected or left the game room!");
		}

		public override void OnDisable() {
			PhotonNetwork.Disconnect();
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps) {
			Debug.LogWarning("Player Properties Has Changed: " + target.ActorNumber.ToString() + " - " + changedProps.Count.ToString());
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
			Debug.LogWarning("Current Room " + this._roomName + " Has Changed!" + propertiesThatChanged.Count.ToString());
		}

		#endregion

		#region CLASS_METHODS
		public override void Init() {
			throw new System.NotImplementedException();
		}

		public void ChangeState(NetworkState state) {
			if(this._currentState == state)
				return;

			this._previousState = this._currentState;
			this._currentState = state;
		}

		public bool ConnectToServer() {

			if(PhotonNetwork.IsConnectedAndReady) {
				Debug.LogWarning("Already Connected To The Master Server");

				this._isConnectedToServer = true;
				this.ChangeState(NetworkState.IN_SERVER);

				MenuManager.instance.ConnectedToServer();

				// Connect to the Master Lobby.
				this.ConnectToLobby();
				return true;
			} else {
				Debug.LogWarning("Connecting To Master Server");

				this.ChangeState(NetworkState.CONNECTING_SERVER);

				if(!PhotonNetwork.ConnectUsingSettings()) {
					Debug.LogWarning("Failed To Connect To the Master Serwver");

					this.OfflineMode();
					return false;
				}
			}

			return true;
		}

		public bool RetryConnectionToServer() {
			if(!this._isConnectedToServer || this._currentState == NetworkState.OFFLINE) {
				Debug.LogWarning("Reconnecting To The Master Server!");
				if(!this.ConnectToServer()) {
					this.OfflineMode();
					return false;
				} else
					return true;
			}

			return true;
		}

		private void ConnectToLobby() {
			if(PhotonNetwork.InLobby) {
				Debug.LogWarning("Already Connected to the Master Lobby");
				this.ChangeState(NetworkState.IN_LOBBY);
			} else {

				Debug.LogWarning("Connecting To the Master Lobby");
				if(!PhotonNetwork.JoinLobby()) {
					if(!this._isConnectedToServer)
						this.ChangeState(NetworkState.OFFLINE);
					else
						this.ChangeState(NetworkState.IN_SERVER);
				}
			}
		}

		public void CreateOrSearchGameRoom() {

			this.ChangeState(NetworkState.CONNECTING_GAMEROOM);

			if(!this._isConnectedToServer || this._currentState == NetworkState.OFFLINE) {

			} else {
				if(!SearchGameRoom()) {
					this.CreateGameRoom();
				}
			}
		}

		public bool CreateGameRoom() {
			if(!this.IsConnectedToServer || this._currentState == NetworkState.OFFLINE) {
				// For offline mode.

			} else {
				if(!PhotonNetwork.CreateRoom("", this._roomOptions, null)) {
					Debug.LogWarning("Failed to Create a new game room.");
					return false;
				}
			}

			this._isMasterClient = true;

			return true;
		}

		public bool SearchGameRoom() {
			if(!PhotonNetwork.JoinRandomRoom()) {
				Debug.LogWarning("Failed to Join a random Game Room, Creating a new room");
				return false;
			}

			return true;
		}

		public void CancelSearchGameRoom() {
			if(this._currentState == NetworkState.WAITING_GAMEROOM || this._currentRoom != null) {
				if(!PhotonNetwork.LeaveRoom()) {
					Debug.LogWarning("Unable to leave the current game room: " + this._currentRoom.Name);
				}
			}
		}

		public void OfflineMode() {
			Debug.LogWarning("Switching To Offline Mode!");

			this._isConnectedToServer = false;
			this.ChangeState(NetworkState.OFFLINE);

			PhotonNetwork.OfflineMode = true;

			MenuManager.instance.OfflineMode();
		}

		public void StartGame() {
			PhotonNetwork.AutomaticallySyncScene = true;

			if(PhotonNetwork.IsMasterClient || this._isMasterClient) {
				PhotonNetwork.LoadLevel(1);
			}
		}
		#endregion
	}
}