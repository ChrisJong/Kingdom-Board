namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Enum;
    using Helpers;
    using Manager;
    using Player;

    [RequireComponent(typeof(NavMeshObstacle))]
    public class GoldMine : EntityBase {

        #region VARIABLE

        [SerializeField] private bool _inControl = false;

        [SerializeField] private int _gold = 1000;

        [SerializeField] private float _radiusCheck = 3.0f;

        [SerializeField] private Player _playerInControl = null;

        private Dictionary<Player, int> _playerUnitCount = new Dictionary<Player, int>();

        [SerializeField] private List<HasHealthBase> _entitiesNear = new List<HasHealthBase>();

        private SphereCollider _triggerCollider = null;

        private NavMeshObstacle _navObstacle = null;

        public StructureClassType classType { get { return StructureClassType.NEUTRAL; } }
        public StructureType structureType { get { return StructureType.GOLDMINE; } }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        public int Gold { get { return this._gold; } }
        public bool InControl { get { return this._inControl; } }
        public Player PlayerInControl { get { return this._playerInControl; } }
        #endregion

        [SerializeField] private Flag[] _flags = new Flag[2];










        #region UNITY

        private void OnTriggerEnter(Collider other) {

            Debug.Log(other.gameObject.name + " Enter Gold Mine");

            HasHealthBase temp = other.GetComponent<HasHealthBase>() as HasHealthBase;
            if(temp != null) {
                this.AddEntity(temp);
            }
        }

        private void OnTriggerExit(Collider other) {

            Debug.Log(other.gameObject.name + " Exit Gold Mine");

            HasHealthBase temp = other.GetComponent<HasHealthBase>() as HasHealthBase;
            if(temp != null) {
                this.RemoveEntity(temp);
            }
        }

        #endregion

        #region CLASS
        public override void Setup() {
            this._triggerCollider = this.transform.GetComponent<SphereCollider>() as SphereCollider;
            this._navObstacle = this.transform.GetComponent<NavMeshObstacle>() as NavMeshObstacle;
        }

        public override void Init() {
            this._triggerCollider.radius = this._radiusCheck;
            this._triggerCollider.isTrigger = true;
            this._navObstacle.carving = true;

            this._inControl = false;
            this._playerInControl = null;
        }

        public void Init(List<Player> players) {

            foreach(Player p in players)
                this._playerUnitCount.Add(p, 0);

            this.Init();
        }

        public override void Return() {

            this._playerInControl = null;

            this._entitiesNear.Clear();
            this._playerUnitCount.Clear();
        }

        public void CheckControl() {

            bool controlStatus = false;
            Player control = null;

            if(this._entitiesNear.Count <= 0) {
                if(this._inControl)
                    DestroyFlags();

                this._inControl = controlStatus;
                this._playerInControl = control;
                return;
            }

            foreach(Player p in this._playerUnitCount.Keys) {

                if(this._playerUnitCount[p] <= 0) // If there are no units for the specific player then continue to the next player.
                    continue;

                if(controlStatus) { // Gold Mine is being contested.
                    controlStatus = false;
                    control = null;
                    if(this._playerInControl != null)
                        DestroyFlags();
                    break;
                }

                // A Player is in control of the gold mine
                controlStatus = true;
                control = p;
            }

            if(control != null && this._playerInControl != null) {
                if(control != this._playerInControl)
                    SpawnFlags((int)control.id);
            } else if(control != null && this._playerInControl == null) {
                SpawnFlags((int)control.id);
            }

            this._inControl = controlStatus;
            this._playerInControl = control;
        }

        public void AddEntity(HasHealthBase entity) {

            if(this._entitiesNear.Contains(entity))
                return;

            this._entitiesNear.Add(entity);

            this._playerUnitCount[entity.controller] += 1;

            /*if(entity.controller == this._playerInControl)
                return;*/

            this.CheckControl();
        }

        public void RemoveEntity(HasHealthBase entity) {

            if(!this._entitiesNear.Contains(entity))
                return;

            if(this._entitiesNear.Count < 0) {
                Debug.LogError("Entity Count Near the Mine Exceeds Negative 0: " + this._entitiesNear.Count.ToString());
                return;
            }

            this._entitiesNear.Remove(entity);

            if(this._playerUnitCount[entity.controller] < 0) {
                Debug.LogError(entity.controller.name + " Entity Count Near the Mine Exceeds Negative 0: " + this._playerUnitCount[entity.controller].ToString());
                return;
            }
            this._playerUnitCount[entity.controller] -= 1;

            this.CheckControl();
        }

        private void SpawnFlags(int controllerId) {
            Debug.Log("Spawn Mine");

            for(int i = 0; i < _flags.Length; i++) {
                _flags[i].OnCapture(controllerId);
            }
        }

        private void DestroyFlags() {
            Debug.Log("Destory Mine");

            for(int i = 0; i < _flags.Length; i++) {
                _flags[i].OnContested();
            }
        }
        #endregion

    }
}