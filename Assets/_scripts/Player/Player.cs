namespace KingdomBoard.Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Research;
    using Structure;
    using UI;
    using Unit;

    public abstract class Player : MonoBehaviour {

        #region VARIABLE
        public uint id;
        public uint roll;
        [Space]
        [SerializeField] protected bool _isAttacking;
        [SerializeField] protected bool _turnEnded;

        protected PlayerCamera _playerCamera;
        protected PlayerSelect _playerSelect;
        protected PlayerSound _playerSound;
        private Research _research;
        protected PlayerUI _playerUI;

        [Space]
        [SerializeField] protected PlayerState _currentState = PlayerState.NONE;
        [SerializeField] protected PlayerState _nextState = PlayerState.NONE;

        private Castle _castle;
        private IList<IStructure> _structures;
        private IList<IUnit> _units;
        private Dictionary<UnitClassType, List<IUnit>> _classUnits;

        private GameObject _structureGroup = null;
        private GameObject _unitGroup = null;
        protected Transform _spawnLocation;
        protected Color _playerColor;

        public bool IsAttacking { get { return this._isAttacking; } }
        public bool TurnEnded { get { return this._turnEnded; } }

        public PlayerCamera playerCamera { get { return this._playerCamera; } }
        public PlayerSelect playerSelect { get { return this._playerSelect; } }
        public PlayerSound playerSound { get { return this._playerSound; } }
        public PlayerUI playerUI { get { return this._playerUI; } set { this._playerUI = value; } }

        public PlayerState CurrentState { get { return this._currentState; } set { this._currentState = value; } }
        public PlayerState NextState { get { return this._nextState; } }

        public Castle castle { get { return this._castle; } }
        public IList<IStructure> structures { get { return this._structures; } }
        public IList<IUnit> units { get { return this._units; } }
        public Dictionary<UnitClassType, List<IUnit>> ClassUnits { get { return this._classUnits; } }

        public GameObject UnitGroup { get { return this._unitGroup; } }
        public GameObject StructureGroup { get { return this._structureGroup; } }
        public Transform SpawnLocation { get { return this._spawnLocation; } }
        public Color PlayerColor { get { return this._playerColor; } set { this._playerColor = value; } }
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
        public virtual void Setup(Transform spawnLocation, uint id = 0) {

            this.id = id;
            this._turnEnded = false;
            this._spawnLocation = spawnLocation;

            this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            this._structureGroup = new GameObject(UIValues.Structure.STRUCTURE_SUFFIX);
            this._structureGroup.transform.SetParent(this.transform);
            this._unitGroup = new GameObject(UIValues.Unit.UNIT_SUFFIX);
            this._unitGroup.transform.SetParent(this.transform);

            ResourceManager.instance.SetupPlayerResources(this);

            this._castle = StructurePoolManager.instance.GetStartCastle(this);

            if(this._research == null) {
                if(this.GetComponent<Research>() == null)
                    this._research = this.gameObject.AddComponent<Research>();
                else
                    this._research = this.GetComponent<Research>();
            }
            this._research.Init(this);
        }

        public virtual void Init(bool attacking) {
            if(attacking) {
                this._isAttacking = attacking;
                this._playerUI.Hide();
                this._currentState = PlayerState.START;
            } else {
                this._isAttacking = false;
                this._playerUI.Hide();
                this._currentState = PlayerState.START;
            }

            this._classUnits = new Dictionary<UnitClassType, List<IUnit>>();
            for(int i = 0; i < System.Enum.GetNames(typeof(UnitClassType)).Length - 2; i++) {
                UnitClassType classType = ((UnitClassType)i + 1);
                this._classUnits.Add(classType, new List<IUnit>());
            }
        }

        public abstract void UpdatePlayer();

        public void SetupNewTurn() {

            this._currentState = PlayerState.START;

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.MainCamera.gameObject.SetActive(true);

            this._playerUI.ShowBanner();
        } 

        public void StartTurn() {

            if(this._isAttacking) {
                this._currentState = PlayerState.WAITING;
                this._nextState = PlayerState.ATTACKING;
                this._playerSelect.CurrentState = SelectionState.FREE;
                GameManager.instance.StartPlayTimer();
            } else {
                this._currentState = PlayerState.DEFENDING;
                this._playerSelect.CurrentState = SelectionState.FREE;
            }

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.MainCamera.gameObject.SetActive(true);
            this.playerUI.Display();
            //this.uiComponent.ShowBanner();

            if(this._isAttacking) {
                this._castle.CheckSpawnQueue();
                this._research.CheckResearchPhase(GameManager.instance.RoundCount);
                this._playerUI.Starthourglass();
            }
        }

        public virtual void NewTurn(bool attacking) {

            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.MainCamera.gameObject.SetActive(false);
            this._castle.castleUI.ResetUI();
            this._playerUI.Hide();

            if(attacking) {
                this._currentState = PlayerState.ATTACKING;
                this._isAttacking = attacking;
            } else {
                this._currentState = PlayerState.DEFENDING;
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

            this._playerSelect.EndTurn();
            this._playerUI.Hide();
            this._turnEnded = true;
            this._playerSelect.CurrentState = SelectionState.END;
            this._currentState = PlayerState.END;


            // (For SinglePlayer) Turn the camera off and hide the ui.
            this._playerCamera.MainCamera.gameObject.SetActive(false);

            this._playerUI.Hide();

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

        public bool ChangeState() {
            if(this._nextState == PlayerState.NONE)
                return false;

            this._currentState = this._nextState;
            this._nextState = PlayerState.NONE;
            return true;
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