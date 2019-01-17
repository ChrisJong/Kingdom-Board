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
        private bool _isAttacking;
        [SerializeField] private bool _turnEnded;
        private Transform _spawnLocation;
        private Color _color;
        [SerializeField] private PlayerState _state = PlayerState.NONE;

        public PlayerState state { get { return this._state; } set { this._state = value; } }

        //////////////////
        //// Entities ////
        //////////////////
        private Castle _castle;
        private IList<IUnit> _units;
        private Dictionary<UnitClassType, List<IUnit>> _classUnits;
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
        protected PlayerUI _ui;

        public PlayerUI UI {
            get { return this._ui; }
            set { this._ui = value; } }

        ///////////////////////
        //// Getter/Setter ////
        ///////////////////////
        public bool isAttacking { get { return this._isAttacking; } }
        public bool turnEnded { get { return this._turnEnded; } }
        public Transform spawnLocation { get { return this._spawnLocation; } }
        public Color color { get { return this._color; } set { this._color = value; } }

        public Castle castle { get { return this._castle; } }
        public IList<IUnit> units { get { return this._units; } }
        public Dictionary<UnitClassType, List<IUnit>> ClassUnits { get { return this._classUnits; } }
        public IList<IStructure> structures { get { return this._structures; } }

        public PlayerCamera playerCamera { get { return this._playerCamera; } }
        public PlayerSelect playerSelection { get { return this._playerSelection; } }
        #endregion

        #region UNITY
        private void OnEnable() {
            // this._color = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));

            this._units = new List<IUnit>();
            this._structures = new List<IStructure>(5);
        }

        private void OnDisable() {
            this._castle = null;
            this._units = null;
            this._structures = null;
        }
        #endregion

        #region CLASS
        public virtual void Create(Transform spawnLocation, uint id = 0) {

            this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            this._unitGroup = new GameObject("_units");
            this._unitGroup.transform.SetParent(this.transform);
            this._structureGroup = new GameObject("_structures");
            this._structureGroup.transform.SetParent(this.transform);

            this.id = id;
            this._name = gameObject.name;
            this._turnEnded = false;

            this._spawnLocation = spawnLocation;

            ResourceManager.instance.SetupPlayerResources(this);

            this._castle = StructurePoolManager.instance.GetStartCastle(this);

            this._research = this.transform.GetComponent<Research>() as Research;
            if(this._research == null)
                this._research = this.gameObject.AddComponent<Research>() as Research;
            this._research.Init(this);
        }

        public virtual void Init(bool attacking) {
            if(attacking) {
                this._isAttacking = attacking;
                this._ui.HideUI();
                this._state = PlayerState.START;
            } else {
                this._isAttacking = false;
                this._ui.HideUI();
                this._state = PlayerState.START;
            }

            this._selectionState = SelectionState.FREE;
            this._playerCamera = PlayerCamera.CreateCamera(this, this._spawnLocation);
            this._playerSelection = this.playerCamera.gameObject.GetComponent<PlayerSelect>();

            this._classUnits = new Dictionary<UnitClassType, List<IUnit>>();
            for(int i = 0; i < System.Enum.GetNames(typeof(UnitClassType)).Length - 2; i++) {
                UnitClassType classType = ((UnitClassType)i + 1);
                this._classUnits.Add(classType, new List<IUnit>());
            }
        }

        public abstract void UpdatePlayer();

        public void SetupNewTurn() {

            this._state = PlayerState.START;

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(true);

            this._ui.ShowBanner();
        } 

        public void StartTurn() {

            if(this._isAttacking) {
                this._state = PlayerState.ATTACKING;
                this._selectionState = SelectionState.FREE;
                GameManager.instance.StartPlayTimer();
            } else {
                this._state = PlayerState.DEFENDING;
                this._selectionState = SelectionState.FREE;
            }

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(true);
            this.UI.DisplayUI();
            //this.uiComponent.ShowBanner();

            if(this._state == PlayerState.ATTACKING) {
                this._castle.CheckSpawnQueue();
                this._research.CheckResearchPhase(GameManager.instance.RoundCount);
            }
        }

        public virtual void NewTurn(bool attacking) {

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(false);
            this._castle.UI.ResetUI();
            this._ui.HideUI();

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
            this._selectionState = SelectionState.END;
            this._state = PlayerState.END;


            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.gameObject.SetActive(false);

            this._ui.HideUI();

            GameManager.instance.StopPlayTimer();
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
            return ReferenceEquals(other.Controller, this);
        }

        public bool IsEnemy(IHasHealth other) {
            return !this.IsAlly(other);
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