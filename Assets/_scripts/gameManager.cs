using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Player;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public MainManager _mainManager;

    public int _numberOfPlayers = 0;

    public int _turnCount;
    public int _currentPlayerID;
    public int[] _turnOrderID;

    public Player.Player[] _playerList;

    public GameObject[] _spawnPoints;

    #region UNITY_METHOD
    private void Awake() {
        if(instance == null)
            instance = this;
        else if(instance != null)
            Destroy(this.gameObject);
    }

    private void Update() {
        this.DebugSwitch();
    }
    #endregion

    #region GAME_MANAGER_METHODS
    public void Init() {
        this._numberOfPlayers = MainManager.instance.NumberOfPlayers;
        this._playerList = new Player.Player[this._numberOfPlayers];

        this.FindCastleSpawn();
        this.SetPlayers();
    }

    private void FindCastleSpawn() {
        this._spawnPoints = GameObject.FindGameObjectsWithTag("CastleSpawn");
    }

    private void SetPlayers() {
        for(int i = 0; i < this._numberOfPlayers; i++) {
            GameObject temp = new GameObject("Player" + (i+1).ToString());

            temp.AddComponent<Player.Player>();

            temp.GetComponent<Player.Player>().init(this._spawnPoints[i], i);
            this._playerList[i] = temp.GetComponent<Player.Player>() as Player.Player;
        }
    }
    #endregion

    #region MAIN_MANAGER_STATIC
    public static void FindOrCreate() {
        GameObject tempManager = GameObject.FindGameObjectWithTag("GameManager");

        if(tempManager == null) {
            tempManager = GameObject.Instantiate(AssetPool.instance.gameManager) as GameObject;
            tempManager.name = "GameManager";
        }
    }
    #endregion

    #region DEBUG
    public int debugID = 0;

    public void DebugSwitch() {
        if(Input.GetKey(KeyCode.F1)) {
            if(this.debugID < this._numberOfPlayers - 1) {
                this._playerList[this.debugID]._camera.GetComponent<Camera>().enabled = false;
                this._playerList[this.debugID+1]._camera.GetComponent<Camera>().enabled = true;
                this.debugID += 1;
            } else {
                this._playerList[this.debugID]._camera.GetComponent<Camera>().enabled = false;
                this._playerList[0]._camera.GetComponent<Camera>().enabled = true;
            }
        }
    }
    #endregion
}
