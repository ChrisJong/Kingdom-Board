namespace Manager {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Enum;
    using Extension;
    using Player;

    public class GameManager : SingletonMono<GameManager> {

        #region VARIABLE
        private int _numberOfPlayers = 2;
        [SerializeField] private int _roundCount = 0;
        [SerializeField] private int _indexOnAttack = 0;
        [SerializeField] private int _indexInView = 0;

        [Space]
        [SerializeField] private float _countdown = 0.0f;
        private float _countdownLimit = 999.0f;

        private IEnumerator _countdownTimer;

        private List<Player> _players = new List<Player>();
        private List<Transform> _spawnPoints = new List<Transform>();

        [Space]
        [SerializeField] private Player _playerOnAttack = null;
        [SerializeField] private Player _playerInView = null;

        [Space]
        public GameObject aiPrefab = null;
        public GameObject humanPrefab = null;

        public int PlayerCount { get { return this._players.Count; } }
        public int RoundCount { get { return this._roundCount; } }

        public float Countdown { get { return this._countdown; } }
        public float ElapsedTime { get { return (this._countdownLimit - this._countdown) / this._countdownLimit; } }

        public List<Player> Players { get { return this._players; } }
        public Player PlayerOnAttack { get { return this._playerOnAttack; } }
        public Player PlayerInView { get { return this._playerInView; } }
        #endregion

        #region UNITY
        private void Start() {
            this.Init();
        }
        #endregion

        #region CLASS
        public override void Init() {
            this.FindSpawnPoints();

            this._countdown = this._countdownLimit;
            this._countdownTimer = this.StartCountdown();

            this.CreatePlayers();

            GoldMineManager.instance.Init();

            this._roundCount++;
            Debug.Log("Round: " + RoundCount.ToString());
            this.NewRound();
        }

        public void CheckRound() {
            int playersFinished = 0;

            foreach(Player player in this._players) {
                if(player.TurnEnded || player.CurrentState == PlayerState.END)
                    playersFinished++;
            }

            if(playersFinished >= this._numberOfPlayers) {
                this.EndRound();
            } else {
                this.NextPlayer(); //return;
            }
        }

        public void StartPlayTimer() {
            StopCoroutine(this._countdownTimer);
            StartCoroutine(this._countdownTimer);
        }

        public void StopPlayTimer() {
            StopCoroutine(this._countdownTimer);
        }

        /// <summary>
        /// Method for Debugging Purposes swtich to the next player and changes the camera.
        /// </summary>
        private void NextPlayer() {           
            // Going out of range.
            
            // Change to the next player in the list.
            this._indexInView++;
            if(this._indexInView == this._numberOfPlayers)
                this._indexInView = 0;

            this._playerInView = this._players[this._indexInView];

            // Enable there camera and Ui and start there turn.
            //this._playerInView.StartTurn();
            this._playerInView.SetupNewTurn();
        }

        private void NewRound() {
            this._countdown = this._countdownLimit;

            foreach(Player player in this._players) {
                if(player == this._playerOnAttack) {
                    player.NewTurn(true);

                    ResourceManager.instance.AddGoldPerTurn(player);
                    player.SetupNewTurn();
                    //player.StartTurn();
                } else {
                    player.NewTurn(false);
                    //p.StartTurn();
                }
            }

            if(Constants.UnitValues.DEATHCOUNTER != 0)
                UnitPoolManager.instance.CountdownDeath();
        }

        private void EndRound() {
            // Change the current attacker to become the defender

            // Change the attacking player to the next person on the list.
            this._indexOnAttack++;
            this._indexInView = this._indexOnAttack;
            if(this._indexOnAttack >= this._numberOfPlayers) {
                this._indexOnAttack = 0;
                this._indexInView = 0;

                this._roundCount++;
                Debug.Log("Round: " + RoundCount.ToString());
            }

            this._playerOnAttack = this._players[this._indexOnAttack];
            this._playerInView = this._players[this._indexOnAttack];

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
                GameObject temp = Instantiate(this.humanPrefab);
                Human player = temp.GetComponent<Human>() as Human;

                if(player == null)
                    player = temp.AddComponent<Human>() as Human;

                temp.name = "Human_" + (i + 1).ToString().PadLeft(2, '0');
                player.roll = (uint)UnityEngine.Random.Range(0, 100);
                this._players.Add(player);
            }

            this._players.Sort((x1, x2) => x2.roll.CompareTo(x1.roll));

            this._indexOnAttack = 0;
            this._indexInView = 0;
            this._playerOnAttack = this._players[0];
            this._playerInView = this._players[0];

            for(int i = 0; i < this._numberOfPlayers; i++) {
                if(i == 0) {
                    this._players[i].PlayerColor = Constants.PlayerValues.WHITE;
                    this._players[i].Setup(this._spawnPoints[i], (uint)i);
                    this._players[i].Init(true);
                } else {
                    this._players[i].PlayerColor = Constants.PlayerValues.BLACK;
                    this._players[i].Setup(this._spawnPoints[i], (uint)i);
                    this._players[i].Init(false);
                }
            }
        }

        private IEnumerator StartCountdown() {
            if(this._countdown <= 0.0f && this._playerInView.TurnEnded) {
                // end the players turns
                this._playerInView.EndTurn();
                this._countdown = this._countdownLimit;
                yield break;
            }

            this._countdown = this._countdownLimit;

            do {

                this._countdown -= Time.deltaTime;

                if(this._playerInView.TurnEnded)
                    break;

                yield return new WaitForEndOfFrame();

            } while(this._countdown > 0.0f && !this._playerInView.TurnEnded);

            this._countdown = 0.0f;

            // End players turn. (Debugging)
            if(this._countdown <= 0.0f)
                this._playerInView.EndTurn();

            /*if(this._countdown <= 0.0f)
                this._playerOnAttack.EndTurn();*/

            this._countdown = this._countdownLimit;

            yield return null;
        }
        #endregion
    }
}