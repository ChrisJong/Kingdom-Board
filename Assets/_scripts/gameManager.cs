using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Player;

public class gameManager : MonoBehaviour {

    // change to private.
    public int numberOfPlayers = 0;
    public int turnCount;

    public int currentTurnID;

    public int castleSpawnCount;

    public mainManager mainManager;

    // combine into a hash/multi array.
    public GameObject[] playersGO;
    public Player.Player[] playersScript;

    public GameObject player;
    public GameObject[] castleSpawn;

    void Awake() {
        this.mainManager = GameObject.FindGameObjectWithTag("mainManager").GetComponent<mainManager>() as mainManager;

        this.FindCastleSpawn();
        this.SetPlayers();
    }

    private void FindCastleSpawn() {
        this.castleSpawn = GameObject.FindGameObjectsWithTag("castleSpawn");
        this.castleSpawnCount = this.castleSpawn.Length;
    }

    private void SetPlayers() {
        this.numberOfPlayers = this.mainManager.NumberOfPlayers;
        this.playersGO = new GameObject[this.numberOfPlayers];
        this.playersScript = new Player.Player[this.numberOfPlayers];

        for (int i = 0; i < this.numberOfPlayers; i++) {

            this.playersGO[i] = new GameObject("Player");
            this.playersScript[i] = this.playersGO[i].AddComponent<Player.Player>();
            this.playersGO[i].name = "Player_" + i.ToString();
            this.playersScript[i].init(this.playersGO[i], this.castleSpawn[i], assetPool.instance.castle, i);
        }

    }
}
