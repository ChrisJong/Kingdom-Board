namespace Unit {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Utility;

    //[RequireComponent(typeof(Rigidbody), typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public abstract class UnitBase : HasHealthBase, IUnit {

        #region VARIABLE
        // Will sort later.
        [Header("UNIT")]
        [SerializeField]
        protected Vector3 _currentPoint = Vector3.zero;
        [SerializeField]
        protected Vector3 _previousPoint = Vector3.zero;
        public Vector3 currentPoint { get { return this._currentPoint; } }
        public Vector3 previousPoint { get { return this._previousPoint; } }

        [SerializeField]
        protected IHasHealth _currentTarget = null;
        [SerializeField]
        protected IHasHealth _previousTarget = null;
        public IHasHealth currentTarget { get { return this._currentTarget; } }
        public IHasHealth previousTarget { get { return this._previousTarget; } }

        [Header("UI")]
        public LineRenderDrawCircle radiusDrawer = null;

        protected RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);
        protected NavMeshAgent _navMeshAgent = null;

        public override EntityType entityType { get { return EntityType.UNIT; } }
        public LayerMask areaMask { get { return this._navMeshAgent.areaMask; } }

        //[SerializeField]
        protected float _unitRadius = 0.0f;
        public float unitRadius { get { return this._unitRadius; } }

        //[SerializeField]
        protected UnitState _unitState = UnitState.NONE;
        public UnitState unitState { get { return this._unitState; } set { this._unitState = value; } }

        ////////////////
        ///// UNIT /////
        ////////////////
        [Header("UNIT")]
        [SerializeField]
        protected UnitType _unitType = UnitType.NONE;
        public UnitType unitType { get { return this._unitType; } }

        [Header("MOVEMENT")]
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        public Vector3 lastPosition { get { return this._lastPosition; } set { this._lastPosition = value; } }
        [Header("MOVEMENT")]
        [SerializeField, ReadOnly]
        protected Vector3 _currentMoveTo = Vector3.zero;
        public Vector3 currentMoveTo { get { return this._currentMoveTo; } set { this._currentMoveTo = value; } }
        [SerializeField, ReadOnly]
        protected Vector3 _previousMoveTo = Vector3.zero;
        public Vector3 previousMoveTo { get { return this._previousMoveTo; } set { this._previousMoveTo = value; } }

        [SerializeField]
        protected MovementType _movementType = MovementType.NONE;
        public MovementType movementType { get { return this.movementType; } }

        [SerializeField, Range(1.0f, 50.0f)]
        protected float _maxStamina = 10.0f;
        protected float _curStamina = 0.0f;
        public float curStamina { get { return this._curStamina; } }

        //[SerializeField]
        protected bool _canMove = true;
        public bool canMove { get { return this._canMove; } }

        [SerializeField, Range(0.0001f, 0.9999f)]
        protected float _minMoveThreashold = 0.01f;
        [SerializeField, Range(1.0f, 50.0f)]
        protected float _moveSpeed = 5.0f;
        public float moveSpeed { get { return this._moveSpeed; } }
        [SerializeField, Range(0.1f, 10.0f)]
        private float _allowedMovementImprecision = 1.0f;

        public bool isMoving { get { return this._velocity.sqrMagnitude > (this._minMoveThreashold * this._minMoveThreashold); } }
        public virtual bool isIdle { get { return !this.isMoving && !this.isDead; } }

        [Header("ATTACK")]
        [SerializeField]
        protected GameObject _projectile = null;

        [SerializeField]
        protected HasHealthBase _target = null;
        protected float _lastAttack = 0.0f;

        //[SerializeField]
        protected bool _canAttack = true;
        public bool canAttack { get { return this._canAttack; } }

        [SerializeField, Range(0.0f, 100.0f)]
        protected float _minDamage = 20.0f;
        public float minDamage { get { return this._minDamage; } }
        [SerializeField, Range(0.0f, 100.0f)]
        protected float _maxDamage = 20.0f;
        public float maxDamage { get { return this._maxDamage; } }
        [SerializeField, Range(1.0f, 50.0f)]
        protected float _attackRadius = 7.5f;
        public float attackRadius { get { return this._attackRadius; } }

        [SerializeField, Range(0.0f, 10.0f)]
        protected float _resistanceMultiplier = 0.5f;
        public float resistancePercentage { get { return this._resistanceMultiplier; } }
        [SerializeField, Range(0.0f, 10.0f)]
        protected float _weaknessMultiplier = 0.5f;
        public float weaknessPercentage { get { return this._weaknessMultiplier; } }

        [SerializeField]
        protected AttackType _resistanceType = AttackType.NONE;
        public AttackType resistanceType { get { return this._resistanceType; } }
        [SerializeField]
        protected AttackType _weaknessType = AttackType.NONE;
        public AttackType weaknessType { get { return this._weaknessType; } }
        [SerializeField]
        protected AttackType _attackType = AttackType.NONE;
        public AttackType attackType { get { return this._attackType; } }
        
        [Header("ANIMATION")]
        [SerializeField]
        protected float _endOfAttack = 0.8f;
        protected Animator _animator = null;
        private AnimationClip _animClip = null;
        private AnimationEvent _animEvent = null;
        #endregion

        #region UNITY
        protected virtual void Awake() {
            this.radiusDrawer.TurnOff();

            this._animator = this.GetComponent<Animator>();
            if(this._animator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            this.SetupAttackEventAnimation();

            if(this._unitType == UnitType.NONE)
                throw new ArgumentException("Unit Type Needs to be Set To Value Other than NONE or ANY");

            this._navMeshAgent = this.GetComponent<NavMeshAgent>();
            this._navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            var collider = this.GetComponent<CapsuleCollider>();
            this._unitRadius = collider != null ? collider.radius : this.GetComponent<SphereCollider>().radius;
        }

        protected override void OnEnable() {
            base.OnEnable();

            this._unitState = UnitState.IDLE;
            this._canAttack = true;
            this._canMove = true;
            this._lastPosition = this.transform.position;

            this._navMeshAgent.speed = this._moveSpeed;
        }

        protected virtual void Update() {
            switch(this._unitState) {
                case UnitState.ATTACK:

                break;

                case UnitState.ATTACK_ANIMATION:
                this.CheckAttackAnimation();
                break;
            }

            //this._velocity = (this.position - this._lastPosition);
            //if(isMoving)
            //Debug.Log("Unit is moving");
            //this._lastPosition = this.position;
        }
        #endregion

        #region CLASS
        //////////////
        //// UNIT ////
        //////////////
        public virtual bool SetPoint(Vector3 point) {
            if(this._currentPoint != null)
                this._previousPoint = this._currentPoint;

            this._currentPoint = point;

            return true;
        }

        public virtual bool SetTarget(IHasHealth target) {
            if(this._currentTarget != null)
                this._previousTarget = this._currentTarget;

            this._currentTarget = target;

            return true;
        }

        protected virtual void UpdateUnit() {
            switch(this._unitState) {

                case UnitState.DEAD:
                Debug.Log(this.name + "IS DEAD!");
                break;

            }
        }

        public virtual void NewTurn() {
            this._unitState = UnitState.IDLE;
            this._canMove = true;
            this._canAttack = true;
        }

        public virtual void Finished() {
            this._unitState = UnitState.FINISH;
        }

        //////////////////
        //// MOVEMENT ////
        //////////////////
        public virtual void MoveTo(Vector3 dest) {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.areaMask)) {
                if((hit.position - this.position).sqrMagnitude < (this._navMeshAgent.stoppingDistance * this._navMeshAgent.stoppingDistance))
                    return; // destination not far enough away.
            }

            this._navMeshAgent.isStopped = false;
            this._navMeshAgent.SetDestination(hit.position);
        }

        public virtual void CancelMove() {
            if(this._lastPosition.Equals(this.position)) {
                this.StopMoving();
                return;
            }

            this._navMeshAgent.enabled = false;
            this.position = this._lastPosition;
            this._navMeshAgent.enabled = true;
            this.StopMoving();
        }

        public virtual void FinishMove() {
            this._canMove = false;
            this.StopMoving();
        }

        public virtual void StopMoving() {
            this._navMeshAgent.isStopped = true;
            this._navMeshAgent.ResetPath();
            this._lastPosition = this.position;
        }

        public void LookAt(Vector3 pos) {
            this.transform.LookAt(new Vector3(pos.x, this.transform.position.y, pos.z), Vector3.up);
        }

        ////////////////
        //// ATTACK ////
        ////////////////
        public float GetDamage() {
            float damage = UnityEngine.Random.Range(this._minDamage, this._maxDamage);
            return Mathf.Round(damage);
        }

        public virtual void Attack() {
            this._unitState = UnitState.ATTACK;

            // NOTE: Spawn Projectile Prefab at if its available.

            this.InternalAttack(this.GetDamage(), this._target);

            ((UI.UnitUI)this._uiComponent).FinishAttack();
            this._canAttack = false;
        }

        public override bool ReceiveDamage(float damage, IHasHealth target) {
            if(this.isDead)
                return true;

            ICanAttack unit = target as ICanAttack;
            float finalDamage = 0.0f;
            float finalmultiplier = 0.0f;

            if(unit.attackType == this.resistanceType)
                finalmultiplier += this.resistancePercentage;
            else if(unit.attackType == this.weaknessType)
                finalmultiplier -= this.weaknessPercentage;

            // NOTE: add multiplier from other sources of damage and armour, also from the envrioment and height.

            finalDamage = Mathf.Round(damage * finalmultiplier);

            this.currentHealth -= finalDamage;
            this.uiComponent.UpdateUI();

            if(this.currentHealth <= 0.0f) {
                if(this.controller != null)
                    this.controller.units.Remove(this);

                UnitPoolManager.instance.Return(this);
                // NOTE: play any death animations, or add in any death effects onto the scene. 

                return true;
            } else {
                // NOTE: play hit anbimation.
            }
            return false;
        }

        protected virtual void InternalAttack(float damage, IHasHealth target) {
            var hits = Utils.hitsBuffers;
            //var pos = this.position + this.transform.forward * this._unitRadius;
            var pos = target.position;
            Physics.SphereCastNonAlloc(pos, this._unitRadius * 2.0f, this.transform.forward, hits, this._attackRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);

            this._hitComparer.position = this.position;
            Array.Sort(hits, this._hitComparer);

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                if(hit.transform == this.transform)
                    continue; // jgnore hits with itself;

                var hasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hasHealth == null || hasHealth.isDead)
                    continue; // ignore anything that doesn't contain health or is dead.

                if(this.IsAlly(hasHealth))
                    continue; // ignore allies.

                hasHealth.lastAttacker = this;
                hasHealth.ReceiveDamage(damage, this as IHasHealth); // only attack the original target chosen.
                break;
            }

            ((UI.UnitUI)this._uiComponent).FinishAttack();
            this._canAttack = false;
        }

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        public virtual void StartAttackAnimation(HasHealthBase target) {
            this._unitState = UnitState.ATTACK_ANIMATION;

            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(target.position);
            this.StopMoving();

            this._target = target;

            this._animator.Play("Attack");
        }

        protected virtual void SetupAttackEventAnimation() {
            this._animEvent = new AnimationEvent();

            if(this._endOfAttack <= 0)
                throw new ArgumentException("End of Attack Animation Timer Needs to be Set, Cannot Be 0");

            this._animEvent.time = this._endOfAttack;
            this._animEvent.functionName = "Attack";

                foreach(AnimationClip clip in this._animator.runtimeAnimatorController.animationClips) {
                    if(clip.name.Contains("Attack")) {
                        this._animClip = clip;
                        break;
                    }
                }
            this._animClip.AddEvent(this._animEvent);
        }

        protected virtual void CheckAttackAnimation() {
            if(this._animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
                if(this._animator.IsInTransition(0)) {
                    //this._unitState = UnitState.ATTACK;
                    this.InternalAttack(this.GetDamage(), this._target);
                }
            }
        }
        #endregion
    }
}