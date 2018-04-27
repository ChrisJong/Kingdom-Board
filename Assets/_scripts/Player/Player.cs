namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Structure;
    using UI;
    using Unit;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        ////////////////
        //// PLAYER ////
        ////////////////
        public uint id;
        public uint roll;
        private string _name;
        private int _currentGold;
        private int _currentUnitCap;
        private int _maxUnitCap;
        private bool _isAttacking;
        private bool _turnEnded;
        private Transform _spawnLocation;
        private Color _color;
        private PlayerState _state = PlayerState.NONE;

        public int CurrentGold { get { return this._currentGold; } }
        public PlayerState state { get { return this._state; } set { this._state = value; } }

        //////////////////
        //// Entities ////
        //////////////////
        public Castle _castle;
        private IList<IUnit> _units;
        private IList<IStructure> _structures;

        ////////////////
        //// Camera ////
        ////////////////
        private PlayerCamera _playerCamera;
        private PlayerSelect _playerSelection;
        [SerializeField]
        private SelectionState _selectionState = SelectionState.NONE;
        
        public SelectionState selectionState { get { return this._selectionState; } set { this._selectionState = value; } }

        ////////////
        //// UI ////
        ////////////
        private ScreenSpaceUI _uiComponent;

        public ScreenSpaceUI uiComponent {
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
            // Random Color for the player and its assocaited entities.
            // NOTE: needs to be changed as a parameter passed on from the game manager.
            this._color = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));

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
            ui.name = UIValues.Player.PLAYERUI;
            this._uiComponent = this.gameObject.AddComponent<PlayerUI>() as ScreenSpaceUI;
            this._uiComponent.controller = this;
        }

        public virtual void Init(bool attacking) {
            this._isAttacking = attacking;

            if(attacking) {
                this._uiComponent.Display();
                this._state = PlayerState.ATTACKING;
            } else {
                this._uiComponent.Hide();
                this._state = PlayerState.DEFENDING;
            }

            this._selectionState = SelectionState.FREE;
            this._playerCamera = PlayerCamera.CreateCamera(this, this._spawnLocation, attacking);
            this._playerSelection = this.playerCamera.gameObject.GetComponent<PlayerSelect>();
        }

        public abstract void UpdatePlayer();
        
        public virtual void NewTurn(bool attacking) {
            this._isAttacking = attacking;
            this._turnEnded = false;
            this._playerCamera.gameObject.SetActive(attacking);
            this.uiComponent.Display();

            if(attacking)
                this._castle.CheckSpawnQueue();

            foreach(IUnit unit in this._units)
                unit.NewTurn();
        }

        public virtual void EndTurn() {
            if(this._turnEnded)
                return;

            this._playerSelection.EndTurn();
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