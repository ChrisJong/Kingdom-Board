namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
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
        private string _name;
        private bool _isAttacking;
        private bool _turnEnded;
        private Transform _spawnLocation;
        private Color _color;
        private int _currentGold;
        private int _currentUnitCap;
        private int _maxUnitCap;

        public int CurrentGold { get { return this._currentGold; } }

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
        private UIComponent _uiComponent;

        public UIComponent uiComponent {
            get { return this._uiComponent; }
            set { this._uiComponent = value; } }

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
        #endregion

        #region UNITY
        private void OnEnable() {
            this._units = new List<IUnit>();
            this._structures = new List<IStructure>(5);

            this._currentGold = PlayerValues.STARTGOLD;
            this._currentUnitCap = 0;
            this._maxUnitCap = PlayerValues.STARTUNITCAP;
        }

        private void OnDisable() {
            this._castle = null;
            this._units = null;
            this._structures = null;
        }
        #endregion

        #region CLASS
        public virtual void Create(Transform spawnLocation, uint id = 0) {
            this.id = id;
            this._name = "Player" + (id + 1).ToString().PadLeft(2, '0');
            this.gameObject.name = this._name;
            this._turnEnded = false;

            this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            this._spawnLocation = spawnLocation;

            this._castle = StructurePoolManager.instance.GetStartCastle(this);

            GameObject ui = GameObject.Instantiate(AssetManager.instance.playerUI, this.transform);
            this._uiComponent = ui.GetComponent<PlayerUI>() as UIComponent;
            ((PlayerUI)this._uiComponent).Init(this);
        }

        public virtual void Init(bool attacking) {
            this._isAttacking = attacking;

            if(attacking)
                this._uiComponent.DisplayUI();
            else
                this._uiComponent.HideUI();

            this._playerCamera = PlayerCamera.CreateCamera(this, this._spawnLocation, attacking);
        }

        public abstract void UpdatePlayer();
        
        public virtual void NewTurn(bool attacking) {
            this._isAttacking = attacking;
            this._turnEnded = false;
            this._playerCamera.gameObject.SetActive(attacking);
            this.uiComponent.isActive = attacking;

            if(attacking)
                this._castle.CheckSpawnQueue();
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

        public void AddResource(int quantity) {
            this._currentGold += quantity;
        }

        public void SpendResource(int quantity) {
            if(this.HasResource(quantity))
                this._currentGold -= quantity;
        }

        public bool HasResource(int quantity) {
            if(this._currentGold < quantity)
                return false;
            return true;
        }

        public bool CheckUnitCap(int cost) {
            int newCap = this._currentUnitCap + cost;

            if(newCap > this._maxUnitCap)
                return false;

            return true;
        }

        public void AddToUnitCap(int cost) {
            this._currentUnitCap += cost;
        }

        public void RemoveFromUnitCap(int cost) {
            this._currentUnitCap -= cost;
        }
        #endregion
    }
}