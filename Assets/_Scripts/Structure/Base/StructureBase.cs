namespace Structure {

    using UnityEngine;
    using UnityEngine.AI;

    using Enum;
    using Helpers;
    using Manager;
    using Player;
    using Scriptable;
    using Utility;

    [RequireComponent(typeof(NavMeshObstacle))]
    public abstract class StructureBase : HasHealthBase, IStructure {
        #region VARIABLE
        [Header("STRUCTURE - DEBUGGING")]
        [ReadOnly] public Vector3 debugCurrentPoint = Vector3.zero;
        [ReadOnly] public Vector3 debugPreviousPoint = Vector3.zero;

        [Header("STRUCTURE")]
        [SerializeField] protected StructureScriptable _structureData;
        protected StructureState _structureState = StructureState.NONE;

        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        protected IHasHealth _currentTarget = null;
        protected IHasHealth _previousTarget = null;

        protected Collider _collider = null;
        protected Bounds _colliderBounds;
        protected NavMeshObstacle _navMeshObstacle = null;

        public StructureScriptable StructureData { get { return this._structureData; } set { this._structureData = value; } }
        public override EntityType entityType { get { return EntityType.STRUCTURE; } }
        public abstract StructureType structureType { get; }
        public StructureState structureState { get { return this._structureState; } set { this._structureState = value; } }

        public Vector3 CurrentPoint { get { return this._currentPoint.Value; } }
        public Vector3 PreviousPoint { get { return this._previousPoint.Value; } }
        public IHasHealth CurrentTarget { get { return this._currentTarget; } }
        public IHasHealth PreviousTarget { get { return this._previousTarget; } }
        #endregion

        #region CLASS
        public override void Setup() {

            this._collider = this.GetComponent<BoxCollider>() as BoxCollider;

            this._navMeshObstacle = this.GetComponent<NavMeshObstacle>() as NavMeshObstacle;

            if(this._collider == null)
                Debug.LogError("Structure Needs a Primative Collider to Carve a path around the navmesh");
            else
                this._navMeshObstacle.carving = true;

            base.Setup();
        }

        public override void Init(Player contoller) {
            base.Init(contoller);

            this._colliderBounds = this._collider.bounds;
            this._collider.enabled = false;
        }

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

            float finalDamage = 0.0f;

            //ICanAttack unit = target as ICanAttack;
            // NOTE: Calculate any resistance and weakness damage here.
            finalDamage = damage;

            this.RemoveHealth(finalDamage);
            Debug.Log(this.name + " Took: " + finalDamage.ToString() + " of Damage From - " + target.gameObject.name);

            if(this.CurrentHealth <= 0.0f) {

                if(this.Controller != null && this.Controller.structures != null)
                    this.Controller.structures.Remove(this);

                // NOTE: spawn in particle effects.
                this.ReturnStructure();

                // NOTE: GAME ENDS HERE, need to flag playerr death/
                //this.controller.state = PlayerState.DEAD;
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