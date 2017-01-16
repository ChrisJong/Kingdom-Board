using UnityEngine;
using System.Collections;

public class mainManager : MonoBehaviour {

    public static mainManager instance = null;

    public gameManager _gameManager = null;

    public enum MainState {
        MENU = 0,
        LOBBY,
        GAME,
        END
    };

    public MainState _currentState = MainState.MENU;

    // change to private.
    public int _numberOfPlayers = 0;

    #region UNITY_METHODS
    private void Awake() {
        if(instance == null)
            instance = this;
        else if(instance != null)
            Destroy(this.gameObject);

        this.SetPlayerCount(2);

        this._currentState = MainState.GAME;
        
    }

    private void Start() {
        this.SetupGameState();
    }

    private void Update() {

    }
    #endregion

    #region GET/SET
    public int NumberOfPlayers {
        get { return this._numberOfPlayers; }
    }
    #endregion

    #region MAIN_MANAGER_METHODS
    public void SetupGameState() {
        if(this._currentState == MainState.GAME) {
            GameObject tempGameManager = GameObject.Instantiate(assetPool.instance.gameManager) as GameObject;
            tempGameManager.name = "GameManager";
            this._gameManager = tempGameManager.GetComponent<gameManager>() as gameManager;
            this._gameManager.Init(this);
        }
    }

    public void SetPlayerCount(int n) {
        this._numberOfPlayers = n;
    }
    #endregion
}