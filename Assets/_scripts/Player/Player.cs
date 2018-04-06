namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Helpers;
    using Manager;
    using Structure;
    using UI;
    using Unit;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        /////////////////////
        //// PLAYER DATA ////
        /////////////////////
        public uint id;
        public uint roll;
        public string name;
        private bool _isAttacking;
        private bool _turnEnded;
        private Transform _spawnLocation;
        private Color _color;

        //////////////////
        //// Entities ////
        //////////////////
        private Castle _castle;
        private IList<IUnit> _units;
        private IList<IStructure> _structures;

        ////////////////
        //// Camera ////
        ////////////////
        private PlayerCamera _playerCamera;
        private PlayerSelect _playerSelection;

        ////////////
        //// UI ////
        ////////////

        ///////////////////////
        //// Getter/Setter ////
        ///////////////////////
        public bool isAttacking { get { return this._isAttacking; } }
        public bool turnEnded { get { return this._turnEnded; } }
        public Transform spawnLocation { get { return this._spawnLocation; } }
        public Color color { get { return this._color; } }

        public Castle castle { get { return this._castle; } }
        public IList<IUnit> units { get { return this._units; } }
        public IList<IStructure> structures { get { return this._structures; } }

        public PlayerCamera playerCamera { get { return this._playerCamera; } }
        public PlayerSelect playerSelection { get { return this._playerSelection; } }

        // UI
        private PlayerUI _playerUi;
        #endregion

        #region UNITY
        private void OnEnable() {
            this._units = new List<IUnit>();
            this._structures = new List<IStructure>(5);

            this._castle = StructurePoolManager.instance.GetStartCastle(this);
        }

        private void OnDisable() {
            this._units = null;
            this._structures = null;
        }
        #endregion

        #region CLASS
        public virtual void Create(Transform spawnLocation, uint id = 0) {
            this.id = id;
            this.name = "Player " + (id + 1).ToString().PadLeft(2, '0');
            this._turnEnded = false;

            this.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

            this._spawnLocation = spawnLocation;

            //this._castle = GameObject.Instantiate(AssetManager.instance.castle, this.PlayerGO.transform.position, this.PlayerGO.transform.rotation, this.PlayerGO.transform);
            //this._castle.GetComponent<Castle>().Init(this);

            GameObject tempUI = GameObject.Instantiate(AssetManager.instance.playerUI, this.transform);
            this._playerUi = tempUI.AddComponent<PlayerUI>();
            this._playerUi.Init(this);
        }

        public virtual void Init(bool attacking) {
            this._isAttacking = attacking;

            if(attacking)
                this._playerUi.DisplayUI();
            else
                this._playerUi.HideUI();

            this._playerCamera = PlayerCamera.CreateCamera(this, this._spawnLocation, attacking);
        }

        public abstract void UpdatePlayer();
        
        public virtual void NewTurn(bool attacking) {
            this._isAttacking = attacking;
            this._turnEnded = false;
            this._playerCamera.gameObject.SetActive(attacking);
            this._playerUi.gameObject.SetActive(attacking);
        }

        public virtual void EndTurn() {
            if(this._turnEnded)
                return;

            this._turnEnded = true;
            GameManager.instance.CheckRound();
        }
        public virtual void OnDeath() {
            int count = this._units.Count;
            for(int i = count - 1; i >= 0; i--)
                UnitPoolManager.instance.Return(this._units[i]);

            count = this._structures.Count;
            for(int i = count - 1; i >= 0; i--)
                StructurePoolManager.instance.Return(this._structures[i]);

                this.gameObject.SetActive(false);
        }

        public bool IsAlly(IHasHealth other) {
            return ReferenceEquals(other.controller, this);
        }

        public bool IsEnemy(IHasHealth other) {
            return !this.IsAlly(other);
        }

        #endregion
    }
}