using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Player;

public class gameManager : MonoBehaviour {

    public static gameManager instance = null;

    public mainManager _mainManager;

    public int _numberOfPlayers = 0;

    public int _turnCount;
    public int _curPlayerTurnID;
    public int[] _turnOrderID;

    public GameObject[] _spawnPoints;

    #region UNITY_METHOD
    private void Awake() {
        if(instance == null)
            instance = this;
        else if(instance != null)
            Destroy(this.gameObject);
    }
    #endregion

    #region GAME_MANAGER_METHODS
    public void Init(mainManager MainManager) {
        this._mainManager = MainManager;

        this._numberOfPlayers = this._mainManager.NumberOfPlayers;
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
            
        }
    }
    #endregion

    #region DEBUG
    public void SwitchPlayers(int id) {

    }
    #endregion
}
