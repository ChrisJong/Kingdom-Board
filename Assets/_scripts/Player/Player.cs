namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Research;
    using Scriptable;
    using Structure;
    using UI;
    using Unit;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        ////////////////
        //// PLAYER ////
        ////////////////
        [Header("PLAYER")]
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
        [SerializeField] private PlayerState _state = PlayerState.NONE;

        public int CurrentGold { get { return this._currentGold; } }
        public int CurrentUnitCap { get { return this._currentUnitCap; } }
        public int MaxUnitCap { get { return this._maxUnitCap; } }
        public PlayerState state { get { return this._state; } set { this._state = value; } }

        //////////////////
        //// Entities ////
        //////////////////
        private Castle _castle;
        private IList<IUnit> _units;
        private Dictionary<ClassType, List<IUnit>> _classUnits;
        private IList<IStructure> _structures;
        private GameObject _unitGroup = null;
        public GameObject unitGroup { get { return this._unitGroup; } }
        private GameObject _structureGroup = null;
        public GameObject structureGroup { get { return this._structureGroup; } }

        private Research _research;

        ////////////////
        //// Camera ////
        ////////////////
        [Header("PLAYER - CAMERA")]
        private PlayerCamera _playerCamera;
        private PlayerSelect _playerSelection;
        [SerializeField]
        private SelectionState _selectionState = SelectionState.NONE;
        
        public SelectionState selectionState { get { return this._selectionState; } set { this._selectionState = value; } }

        ////////////
        //// UI ////
        ////////////
        private PlayerUI _uiComponent;

        public PlayerUI uiComponent {
            get { return this._uiComponent; }
            set { this._uiComponent = value; } }

        ///////////////////////
        //// Getter/Setter ////
        ///////////////////////
        public bool isAttacking { get { return this._isAttacking; } }
        public bool turnEnded { get { return this._turnEnded; } }
        public Transform spawnLocation { get { return this._spawnLocation; } }
        public Color color { get { return this._color; } set { this._color = value; } }

        public Castle castle { get { return this._castle; } }
        public IList<IUnit> units { get { return this._units; } }
        public Dictionary<ClassType, List<IUnit>> ClassUnits { get { return this._classUnits; } }
        public IList<IStructure> structures { get { return this._structures; } }

        public PlayerCamera playerCamera { get { return this._playerCamera; } }
        public PlayerSelect playerSelection { get { return this._playerSelection; } }
        #endregion

        #region UNITY
        private void OnEnable() {
            // this._color = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));

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
            this._unitGroup = new GameObject("_units");
            this._unitGroup.transform.SetParent(this.transform);
            this._structureGroup = new GameObject("_structures");
            this._structureGroup.transform.SetParent(this.transform);

            this.id = id;
            this._name = gameObject.name;
            this._turnEnded = false;

            this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            this._spawnLocation = spawnLocation;

            this._castle = StructurePoolManager.instance.GetStartCastle(this);

            GameObject ui = this.transform.Find(UIValues.UISUFFIX).gameObject;
            this._uiComponent = this.gameObject.AddComponent<PlayerUI>();
            this._uiComponent.controller = this;

            this._research = this.transform.GetComponent<Research>() as Research;
            if(this._research == null)
                this._research = this.gameObject.AddComponent<Research>() as Research;
            this._research.Init(this);
        }

        public virtual void Init(bool attacking) {
            if(attacking) {
                this._isAttacking = attacking;
                this._uiComponent.Hide();
                this._state = PlayerState.ATTACKING;
            } else {
                this._isAttacking = false;
                this._uiComponent.Hide();
                this._state = PlayerState.DEFENDING;
            }

            this._selectionState = SelectionState.FREE;
            this._playerCamera = PlayerCamera.CreateCamera(this, this._spawnLocation);
            this._playerSelection = this.playerCamera.gameObject.GetComponent<PlayerSelect>();

            this._classUnits = new Dictionary<ClassType, List<IUnit>>();
            for(int i = 0; i < System.Enum.GetNames(typeof(ClassType)).Length - 2; i++) {
                ClassType classType = ((ClassType)i + 1);
                this._classUnits.Add(classType, new List<IUnit>());
            }
        }

        public abstract void UpdatePlayer();

        public void StartTurn() {

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(true);

            this.uiComponent.Display();

            if(this._state == PlayerState.ATTACKING) {
                this._castle.CheckSpawnQueue();
                this._research.CheckResearchPhase(GameManager.instance.RoundCount);
            }
        }

        public virtual void NewTurn(bool attacking) {

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(false);

            this._uiComponent.Hide();

            if(attacking) {
                this._state = PlayerState.ATTACKING;
                this._isAttacking = attacking;
            } else {
                this._state = PlayerState.DEFENDING;
                this._isAttacking = false;
            }

            this._turnEnded = false;

            for(int i = 0; i < this._units.Count; i++) {
                this._units[i].NewTurn();
            }
        }

        public virtual void EndTurn() {
            if(this._turnEnded)
                return;

            this._playerSelection.EndTurn();
            this._turnEnded = true;

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(false);

            this._uiComponent.Hide();

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
                this._currentGold = this._currentGold - quantity;
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

        public void AddUnit(IUnit unit) {
            if(!this._units.Contains(unit))
                this._units.Add(unit);

            if(this._classUnits.ContainsKey(unit.classType))
                if(!this._classUnits[unit.classType].Contains(unit))
                    this._classUnits[unit.classType].Add(unit);

            // Apply any Research upgrade to this new unit.
            this._research.ApplyUpgrades(unit);
        }

        public void RemoveUnit(IUnit unit) {
            if(this._units.Contains(unit))
                this._units.Remove(unit);

            if(this._classUnits.ContainsKey(unit.classType))
                if(this._classUnits[unit.classType].Contains(unit))
                    this._classUnits[unit.classType].Remove(unit);
        }
        #endregion
    }
}