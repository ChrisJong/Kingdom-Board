namespace Manager {

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Extension;
    using Player;
    using Utility;

    public class GameManager : SingletonMono<GameManager> {

        #region VARIABLE
        public int RoundCount { get; private set; }
        private int _indexOnAttack = 0;
        private int _indexInView = 0;
        private int _numberOfPlayers = 2;

        public AssetManager _assetManager;

        public Player PlayerInView { get; private set; }
        public Player PlayerOnAttack { get; private set; }

        public List<Player> _players;
        private List<Transform> _spawnPoints;

        public List<Player> players { get { return this._players; } }
        #endregion

        #region UNITY
        public void OnDisable() {
            this.PlayerInView = null;
            this.PlayerOnAttack = null;
            this._players = null;
            this._spawnPoints = null;
        }

        protected override void Awake() {
            base.Awake();

            this._players = new List<Player>();
            this._spawnPoints = new List<Transform>();

            this.FindSpawnPoints();
        }

        private void Start() {
            this._assetManager = AssetManager.instance;
            if(this._assetManager == null)
                Debug.LogError("ASSET MANAGER IS MISSING PLEASE ADD ONE");

            this.CreatePlayers();
        }
        #endregion

        #region CLASS
        private void NewRound() {
            this.RoundCount++;
            Debug.Log("Round: " + RoundCount.ToString());

            foreach(Player p in this._players) {
                if(p == this.PlayerOnAttack)
                    p.NewTurn(true);
                else
                    p.NewTurn(false);
            }
        }

        public void CheckRound() {
            if(!this.PlayerOnAttack.turnEnded) {
                return;
            } else {
                if(!CheckPlayerOrder()) {
                    this.EndRound();
                } else {
                    this.NextPlayer();
                }
            }
        }

        private bool CheckPlayerOrder() {
            var count = this._indexOnAttack + 1;
            if(count > this._numberOfPlayers - 1)
                return false; // start a new round.
            else 
                return true; // next player in the lists turn.
        }

        private void NextPlayer() {
            this._indexOnAttack += 1;
            this._indexInView = this._indexOnAttack;
            this.PlayerOnAttack = this._players[this._indexOnAttack];
            this.PlayerInView = this.PlayerOnAttack;

            foreach(Player p in this._players) {
                if(p == this.PlayerOnAttack)
                    p.NewTurn(true);
                else
                    p.NewTurn(false);
            }
        }

        private void EndRound() {
            this._indexOnAttack = 0;
            this._indexInView = 0;
            this.PlayerOnAttack = this._players[this._indexOnAttack];
            this.PlayerInView = this.PlayerOnAttack;

            this.NewRound();
        }

        private void FindSpawnPoints() {
            Transform[] spawnGroup = GameObject.FindGameObjectWithTag("Spawn").GetComponentsInParent<Transform>();

            if(spawnGroup.Length == 0)
                Debug.LogError("No Spawn Points, Please Create Some And Tag It With Spawn - Spawns: " + spawnGroup.Length);

            if(spawnGroup.Length > this._numberOfPlayers)
                Debug.LogError("Not Enough Spawns For the Amount Of Players Playing - Spawns: " + spawnGroup.Length + " - Players: " + this._numberOfPlayers);

            foreach(GameObject trans in GameObject.FindGameObjectsWithTag("Spawn")) {
                this._spawnPoints.Add(trans.GetComponent<Transform>());
            }
        }

        private void CreatePlayers() {
            for(int i = 0; i < this._numberOfPlayers; i++) {
                GameObject temp = new GameObject("Player" + (i + 1).ToString());
                var player = temp.AddComponent<Human>() as Human;
                player.Create(this._spawnPoints[i], (uint)i);
                player.roll = (uint)Random.Range(0, 100);
                //Debug.Log(player.name + " Rolled: " + player.roll.ToString());
                this._players.Add(player);
            }

            this._players.Sort((x1, x2) => x2.roll.CompareTo(x1.roll));

            this._indexOnAttack = 0;
            this._indexInView = 0;
            this.PlayerOnAttack = this._players[0];
            this.PlayerInView = this._players[0];

            for(int i = 0; i < this._numberOfPlayers; i++) {
                this._players[i].Init((i == this._indexOnAttack) ? true : false);

                if(this.players[i] == this.PlayerOnAttack)
                    this.players[i].color = new Color(0.8823529f, 0.8823529f, 0.8823529f, 1.0f);
                else
                    this.players[i].color = new Color(0.2352941f, 0.2352941f, 0.2352941f, 1.0f);
            }

            this.RoundCount++;
            Debug.Log("Round: " + RoundCount.ToString());
        }
        #endregion
    }
}