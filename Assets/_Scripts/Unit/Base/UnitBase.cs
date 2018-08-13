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
        [Header("UI")]
        public LineRenderDrawCircle radiusDrawer = null;

        protected RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);
        protected NavMeshAgent _navMeshAgent = null;

        public override EntityType entityType { get { return EntityType.UNIT; } }
        public LayerMask areaMask { get { return this._navMeshAgent.areaMask; } }

        ////////////////
        ///// UNIT /////
        ////////////////
        [Header("UNIT")]
        [SerializeField]
        protected UnitType _unitType = UnitType.NONE;
        public UnitType unitType { get { return this._unitType; } }
        [SerializeField]
        protected UnitState _unitState = UnitState.NONE;
        public UnitState unitState { get { return this._unitState; } set { this._unitState = value; } }
        [SerializeField]
        protected float _unitRadius = 0.0f;
        public float unitRadius { get { return this._unitRadius; } }

        [ReadOnly]
        public Vector3 debugCurrentPoint = Vector3.zero;
        [ReadOnly]
        public Vector3 debugPreviousPOint = Vector3.zero;
        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        public Vector3 currentPoint { get { return this._currentPoint.Value; } }
        public Vector3 previousPoint { get { return this._previousPoint.Value; } }

        [SerializeField]
        protected IHasHealth _currentTarget = null;
        [SerializeField]
        protected IHasHealth _previousTarget = null;
        public IHasHealth currentTarget { get { return this._currentTarget; } }
        public IHasHealth previousTarget { get { return this._previousTarget; } }

        [Header("MOVEMENT")]
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Vector3 _resetPosition = Vector3.zero;
        public Vector3 lastPosition { get { return this._lastPosition; } set { this._lastPosition = value; } }
        protected NavMeshPath _unitPathing;
        [Header("MOVEMENT")]
        [SerializeField]
        protected float _initialDistance = 0.0f;
        [SerializeField]
        protected MovementType _movementType = MovementType.NONE;
        public MovementType movementType { get { return this.movementType; } }

        [SerializeField, Range(1.0f, 50.0f)]
        protected float _maxStamina = 10.0f;
        protected float _currentStamina = 0.0f;
        [SerializeField, ReadOnly]
        protected float _debugCurrentStamina = 0.0f;
        public float currentStamina { get { return this._currentStamina; } }

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
        protected GameObject _projectileReleasePoint = null;
        [SerializeField]
        protected float _projectileSpeed = 10.0f;
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

            //this.SetupAttackEventAnimation();

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

            this._currentStamina = this._maxStamina;
            this._debugCurrentStamina = this.currentStamina;

            this._lastPosition = this.transform.position;
            this._navMeshAgent.speed = this._moveSpeed;
        }

        protected override void OnDisable() {
            base.OnDisable();

            this._unitState = UnitState.NONE;
            this._canAttack = false;
            this._canMove = false;

            this._lastPosition = Vector3.zero;
            this._navMeshAgent.speed = 0;
        }

        protected virtual void Update() {
            switch(this._unitState) {
                case UnitState.ATTACK_ANIMATION:
                //this.CheckAttackAnimation();
                break;

                case UnitState.MOVING:
                this.CheckMovement();
                break;
            }
        }
        #endregion

        #region CLASS
        //////////////
        //// UNIT ////
        //////////////
        public virtual bool SetPoint(Vector3 point) {
            Vector3 position = Vector3.zero;

            if(!Utils.SamplePosition(point, out position)) {
                return false;
            }

            if(this._currentPoint.HasValue) {
                this._previousPoint = this._currentPoint;
                this.debugPreviousPOint = this._previousPoint.Value;
            }

            this._currentPoint = position;
            this.debugCurrentPoint = position;

            if(this._previousPoint.HasValue && this._currentPoint.HasValue) {
                if(this._currentPoint.Value.Equals(this._previousPoint.Value))
                    return false;
            }

            if(this._unitState == UnitState.MOVING_STANDBY || this.unitState == UnitState.MOVING) {
                this.MoveTo(position);
            }

            return true;
        }

        public virtual bool SetTarget(IHasHealth target) {

            if(this._currentTarget != null)
                this._previousTarget = this._currentTarget;

            this._currentTarget = target;

            if(this._unitState == UnitState.ATTACK_STANDBY) {
                float distance = Vector3.Distance(this.position, target.position);

                if(!this.IsEnemy(target) || (distance + this.unitRadius) > this.attackRadius) {
                    return false;
                } else {
                    this.StartAttackAnimation();
                }
            }

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
            this._lastPosition = this.position;
            this._resetPosition = this.position;
        }

        public virtual void Finished() {
            this._unitState = UnitState.FINISH;
        }

        //////////////////
        //// MOVEMENT ////
        //////////////////
        #region MOVEMENT
        public virtual void MoveTo(Vector3 dest) {
            //NavMeshHit hit;
            NavMeshPath path = new NavMeshPath();

            this.StopMoving();

            /*if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.areaMask)) {
                if((hit.position - this.position).sqrMagnitude < (this._navMeshAgent.stoppingDistance * this._navMeshAgent.stoppingDistance))
                    return; // destination not far enough away.
            }*/

            this._navMeshAgent.CalculatePath(dest, path);
            this._navMeshAgent.SetPath(path);
            this._unitPathing = path;

            if(this._unitPathing.status == NavMeshPathStatus.PathComplete) {
                this._initialDistance = this._navMeshAgent.remainingDistance;
                this._navMeshAgent.isStopped = false;
                this.lastPosition = this.position;
            } else {
                this._navMeshAgent.SetDestination(dest);
                this._initialDistance = this._navMeshAgent.remainingDistance;
            }

            this._unitState = UnitState.MOVING;
        }

        public virtual void CancelMove() {
            if(this._lastPosition.Equals(this.position)) {
                this.StopMoving();
                return;
            }

            this._navMeshAgent.enabled = false;
            this.position = this._resetPosition;
            this._navMeshAgent.enabled = true;
            this.StopMoving();
            this._unitState = UnitState.IDLE;
        }

        public virtual void FinishMove() {
            this._canMove = false;
            this._unitState = UnitState.IDLE;
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

        private bool SamplePosition(Vector3 dest, out Vector3 position) {
            NavMeshHit hit;

            if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.areaMask)) {
                position = hit.position;
                return true;
            }else {
                position = Vector3.zero;
                return false;
            }
        }

        private void CheckMovement() {
            // Seocondary Code to calculate the remainging distance. incase the one on the bottom doesnt work.
            /*float distance = 0.0f;
            Vector3[] corners = this._navMeshAgent.path.corners;
            for(int c = 0; c < corners.Length - 1; ++c) {
                distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);

            }*/
            if(this.currentStamina <= 0.0f) {
                this._canMove = false;
                this._unitState = UnitState.IDLE;
                ((UI.UnitUI)this._uiComponent).FinishMove();
                this.StopMoving();
            }

            Debug.Log("Remaining Distance: " + this._navMeshAgent.remainingDistance.ToString());
            if(this._navMeshAgent.remainingDistance <= this._navMeshAgent.stoppingDistance) {
                if(!this._navMeshAgent.hasPath || this._navMeshAgent.velocity.sqrMagnitude == 0.0f) {
                    this._unitState = UnitState.MOVING_STANDBY;
                }
            } else {

                if(this._navMeshAgent.remainingDistance == this._initialDistance)
                    return;

                // Stamina Calculations go here.

                //this._currentStamina = Mathf.Clamp(this._currentStamina - (this._navMeshAgent.remainingDistance * 0.5f), 0.0f, this._maxStamina);
                //this._debugCurrentStamina = this._currentStamina;
            }
        }
        #endregion

        ////////////////
        //// ATTACK ////
        ////////////////
        #region ATTACK
        public virtual void Attack() {
            if(this._projectile == null) {
                this._unitState = UnitState.ATTACK;
                this.InternalAttack(this.GetDamage());
            } else {
                GameObject temp = Instantiate(this._projectile);

                if(temp.GetComponent<Projectile>() == null)
                    temp.AddComponent<Projectile>();

                temp.GetComponent<Projectile>().SetupTarget(this.gameObject, this._currentTarget.gameObject.GetComponent<Collider>().transform, this._projectileReleasePoint.transform, this._projectileSpeed);
            }
        }

        public virtual void ProjectileAttack() {
            this._unitState = UnitState.ATTACK;
            this.InternalAttack(this.GetDamage());
        }

        public float GetDamage() {
            float damage = UnityEngine.Random.Range(this._minDamage, this._maxDamage);
            return Mathf.Round(damage);
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

            this.RemoveHealth(finalDamage);
            Debug.Log("Current Target (" + this.name + "): Took" + finalDamage.ToString() + " of Damage from - " + target.gameObject.name);
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

        protected virtual void InternalAttack(float damage) {
            this._currentTarget.lastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth);

            ((UI.UnitUI)this._uiComponent).FinishAttack();
            this._unitState = UnitState.IDLE;
            this._canAttack = false;
        }
        #endregion

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        #region ANIMATION
        public void InitialSetupAnimation() {
            this._animator = this.GetComponent<Animator>();
            if(this._animator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            this.SetupAttackEventAnimation();
        }

        public virtual void StartAttackAnimation() {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(this._currentTarget.position);
            this.StopMoving();

            this._unitState = UnitState.ATTACK_ANIMATION;
            this._animator.Play("Attack");
        }

        protected virtual void SetupAttackEventAnimation() {
            this._animEvent = new AnimationEvent();

            if(this._endOfAttack <= 0.0f)
                throw new ArgumentException("End of Attack Animation Timer Needs to be Set, Cannot Be 0");

            this._animEvent.time = this._endOfAttack;
            this._animEvent.functionName = "Attack";

            foreach(AnimationClip clip in this._animator.runtimeAnimatorController.animationClips) {
                if(clip.name.Contains("Attack")) {
                    this._animClip = clip;
                    
                    // Function To Help Find Frame Event Time.
                    /*foreach(AnimationEvent evt in clip.events) {
                        Debug.Log("Event Attack Time: " + evt.time);
                    }*/

                    break;
                }
            }

            this._animClip.AddEvent(this._animEvent);
        }
        #endregion
        #endregion
    }
}