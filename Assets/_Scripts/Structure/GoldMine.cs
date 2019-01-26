namespace Structure {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Enum;
    using Helpers;
    using Manager;

    [RequireComponent(typeof(NavMeshObstacle))]
    public class GoldMine : EntityBase {

        [SerializeField] private float _radiusCheck = 3.0f;

        [SerializeField] private int _gold = 1000;
        [SerializeField] private int _entityCount = 0;
        [SerializeField] private int _playerCount = 0;
        [SerializeField] private int _inControl = -1; // -1 No one in control of this mine.
        [SerializeField] private int[] _controlCount;

        [SerializeField] private List<HasHealthBase> _entitiesNear = new List<HasHealthBase>();

        [SerializeField] private SphereCollider _triggerCollider = null;

        [SerializeField] private NavMeshObstacle _navObstacle = null;

        public StructureClassType classType { get { return StructureClassType.NEUTRAL; } }
        public StructureType structureType { get { return StructureType.GOLDMINE; } }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        public int Gold { get { return this._gold; } }
        public int InContol { get { return this._inControl; } }

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
            this._playerCount = GameManager.instance.PlayerCount;
            this._controlCount = new int[this._playerCount];

            for(int i = 0; i < this._playerCount; i++)
                this._controlCount[i] = 0;
        }

        public override void Return() {

            this._inControl = -1;

            this._entitiesNear.Clear();
        }

        public void CheckControl() {

            if(this._entityCount <= 0 || this._entitiesNear.Count <= 0)
                return;

            bool contolStatus = false;
            int controllerID = -1;

            for(int i = 0; i < this._playerCount; i++) {

                if(this._controlCount[i] <= 0)
                    continue;

                if(contolStatus == true) {
                    contolStatus = false;
                    controllerID = -1;
                    break;
                } else {
                    contolStatus = true;
                    controllerID = i;
                }
            }

            this._inControl = controllerID;
        }

        public void AddEntity(HasHealthBase entity) {

            if(this._entitiesNear.Contains(entity))
                return;

            this._entitiesNear.Add(entity);
            this._entityCount++;
            this._controlCount[entity.controller.id] += 1;

            this.CheckControl();

        }

        public void RemoveEntity(HasHealthBase entity) {

            if(!this._entitiesNear.Contains(entity))
                return;

            this._entitiesNear.Remove(entity);
            this._entityCount--;
            this._controlCount[entity.controller.id] -= 1; 

            if(this._entityCount < 0) {
                Debug.LogError("Entity Count Near the Mine Exceeds Negative 0: " + this._entitiesNear.Count.ToString());
            }

            this.CheckControl();
        } 
        #endregion

    }
}