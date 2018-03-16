namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Extension;
    using Player;

    public class GameManager : SingletonMono<GameManager> {

        #region VARIABLE
        public int RoundCount { private set; get; }
        private int _idOnAttack = 0;
        private int _indexOrder = 0;
        private int[] _playerOrder;
        private int _numberOfPlayers = 2;

        public AssetManager _assetManager;

        public List<Player> _players;
        public List<Transform> _spawnPoints;

        public Player PlayerOnAttack { get; private set; }

        #endregion

        #region UNITY
        protected override void Awake() {
            base.Awake();

            this._players = new List<Player>();
            this._playerOrder = new int[this._numberOfPlayers];
            this._spawnPoints = new List<Transform>();

            this.FindSpawnPoints();
        }

        private void Start() {
            this._assetManager = AssetManager.instance;
            if(this._assetManager == null)
                Debug.LogError("ASSET MANAGER IS MISSING PLEASE ADD ONE");

            this.CreatePlayers();
        }

        private void FixedUpdate() {

        }
        #endregion

        #region CLASS
        private void NewRound() {

        }

        private void CheckRound() {

        }

        private void EndRound() {

        }

        private void FindSpawnPoints() {
            Transform[] spawnGroup = GameObject.FindGameObjectWithTag("Spawn").GetComponentsInParent<Transform>();
            Debug.Log("Spawn Point: " + spawnGroup.Length);
            if(spawnGroup.Length == 0)
                Debug.LogError("No Spawn Points, Please Create Some And Tag It With Spawn - Spawns: " + spawnGroup.Length);

            if(spawnGroup.Length > this._numberOfPlayers)
                Debug.LogError("Not Enough Spawns For the Amount Of Players Playing - Spawns: " + spawnGroup.Length + " - Players: " + this._numberOfPlayers);

            foreach(GameObject trans in GameObject.FindGameObjectsWithTag("Spawn")) {
                this._spawnPoints.Add(trans.GetComponent<Transform>());
            }
        }

        private void CreatePlayers() {
            bool attacking;
            this._idOnAttack = Random.Range(0, this._numberOfPlayers);

            for(int i = 0; i < this._numberOfPlayers; i++) {
                if(i == this._idOnAttack) {
                    attacking = true;
                }else {
                    attacking = false;
                }

                GameObject temp = new GameObject("Player" + (i + 1).ToString());

                var player = temp.AddComponent<Human>() as Human;
                player.Init(temp, this._spawnPoints[i], i, attacking);
                this._players.Add(player);

                if(attacking)
                    this.PlayerOnAttack = player;
            }
        }
        #endregion
    }
}