using UnityEngine;
using System.Collections;

public class MainManager : MonoBehaviour {

    public static MainManager instance = null;

    public enum MainState {
        MENU = 0,
        LOBBY,
        START,
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

        this._currentState = MainState.START;
        
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
        if(this._currentState == MainState.START) {
            GameManager.FindOrCreate();
            GameManager.instance.Init();

            this._currentState = MainState.GAME;
        }
    }

    public void SetPlayerCount(int n) {
        this._numberOfPlayers = n;
    }
    #endregion

    #region MAIN_MANAGER_STATIC
    public static void FindOrCreate() {
        GameObject tempManager = GameObject.FindGameObjectWithTag("MainManager");

        if(tempManager == null) {
            tempManager = GameObject.Instantiate(AssetPool.instance.mainManager) as GameObject;
            tempManager.name = "MainManager";
        }
    }
    #endregion
}