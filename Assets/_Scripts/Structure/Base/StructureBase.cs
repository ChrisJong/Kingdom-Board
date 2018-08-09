namespace Structure {

    using System;

    using UnityEngine;
    using UnityEngine.AI;

    using Enum;
    using Helpers;
    using Manager;
    using Utility;

    [RequireComponent(typeof(UnityEngine.AI.NavMeshObstacle))]
    public abstract class StructureBase : HasHealthBase, IStructure {
        #region VARIABLE
        public abstract StructureType structureType { get; }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }

        protected NavMeshObstacle _navMeshObstacle = null;

        [Header("STRUCTURE")]
        protected StructureState _structureState = StructureState.NONE;
        public StructureState structureState { get { return this._structureState; } set { this._structureState = value; } }

        [Header("SELECTION")]
        [ReadOnly]
        public Vector3 debugCurrentPoint = Vector3.zero;
        [ReadOnly]
        public Vector3 debugPreviousPoint = Vector3.zero;
        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        public Vector3 currentPoint { get { return this._currentPoint.Value; } }
        public Vector3 previousPoint { get { return this._previousPoint.Value; } }

        [Header("SELECTION")]
        [ReadOnly]
        public IHasHealth debugCurrentTarget = null;
        [ReadOnly]
        public IHasHealth debugPreviousTarget = null;
        protected IHasHealth _currentTarget = null;
        protected IHasHealth _previousTarget = null;
        public IHasHealth currentTarget { get { return this._currentTarget; } }
        public IHasHealth previousTarget { get { return this._previousTarget; } }

        public bool isReady { get; set; }
        #endregion

        #region UNITY
        protected virtual void Awake() {
            this._navMeshObstacle = this.GetComponent<NavMeshObstacle>() as NavMeshObstacle;
            this._navMeshObstacle.carving = true;
        }

        protected override void OnEnable() {
            base.OnEnable();
            this.currentHealth = this._maxHealth;
        }
        #endregion

        #region CLASS
        public virtual bool SetPoint(Vector3 point) {
            Vector3 position = Vector3.zero;

            if(!Utils.SamplePosition(point, out position)) {
                return false;
            }

            if(this._currentPoint.HasValue) {
                this._previousPoint = this._currentPoint;
                this.debugPreviousPoint = this._previousPoint.Value;
            }

            this._currentPoint = position;
            this.debugCurrentPoint = position;

            if(this._previousPoint.HasValue && this._currentPoint.HasValue) {
                if(this._currentPoint.Value.Equals(this._previousPoint.Value))
                    return false;
            }

            if(this._structureState == StructureState.STANDBY_POINT){
                // do something.
            }

            return true;
        }

        public virtual bool SetTarget(IHasHealth target) {
            if(this._currentTarget != null)
                this._previousTarget = this._currentTarget;

            this._currentTarget = target;

            if(this._structureState == StructureState.STANDBY_TARGET) {

                if(!this.IsEnemy(target)) {
                    return false;
                }else {

                }
            }

            return true;
        }

        private bool SamplePosition(Vector3 dest, out Vector3 position) {
            NavMeshHit hit;

            if(NavMesh.SamplePosition(dest, out hit, 1.0f, NavMesh.AllAreas)) {
                position = hit.position;
                return true;
            } else {
                position = Vector3.zero;
                return false;
            }
        }

        public override bool ReceiveDamage(float damage, IHasHealth target) {
            if(this.isDead)
                return true;

            //var lookUp = Quaternion.LookRotation(Vector3.up);

            this.currentHealth -= damage;
            if(this.currentHealth <= 0.0f) {

                if(this.controller != null && this.controller.structures != null)
                    this.controller.structures.Remove(this);

                // NOTE: spawn in particle effects.
                this.ReturnStructure();
                return true;
            }
            return false;
        }

        private void ReturnStructure() {
            StructurePoolManager.instance.Return(this);
        }
        #endregion
    }
}