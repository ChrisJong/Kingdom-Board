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
        //////////////
        //// UNIT ////
        //////////////
        public LineRenderDrawCircle radiusDrawer;
        protected Animator _animator;
        protected float _unitRadius;
        protected bool _hasFinished;
        private NavMeshAgent _agent;

        public abstract UnitType unitType { get; }
        public override EntityType entityType { get { return EntityType.UNIT; } }
        public LayerMask areaMask { get { return this._agent.areaMask; } }
        public float unitRadius { get { return this._unitRadius; } }
        public bool hasFinished { get { return this._hasFinished; } }

        //////////////////
        //// MOVEMENT ////
        //////////////////
        protected float _minMoveThreashold = 0.01f;
        protected float _moveSpeed = 5.0f;
        protected float _moveRadius = 10.0f;
        protected bool _canMpve;
        [SerializeField, Range(0.1f, 10.0f)]
        private float _allowedMovementImprecision = 1.0f;
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        public abstract MovementType movementType { get; }
        public bool isMoving { get { return this._velocity.sqrMagnitude > (this._minMoveThreashold * this._minMoveThreashold); } }
        public virtual bool isIdle { get { return !this.isMoving && !this.isDead; } }
        public bool canMove { get { return this._canMpve; } }
        public float moveSpeed { get { return this._moveSpeed; } }
        public float moveRadius { get { return this._moveRadius; } }

        ////////////////
        //// ATTACK ////
        ////////////////
        protected float _minDamage = 10.0f;
        protected float _maxDamage = 20.0f;
        protected float _attackRadius = 7.5f;
        protected float _lastAttack;
        protected float _resistancePercentage = 50.0f;
        protected float _weaknessPercentage = 50.0f;
        protected bool _canAttack;
        protected bool _isAttacking;
        private RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);

        public float minDamage { get { return this._minDamage; } }
        public float maxDamage { get { return this._maxDamage; } }
        public float attackRadius { get { return this._attackRadius; } }
        public float resistancePercentage { get { return this._resistancePercentage; } }
        public float weaknessPercentage { get { return this._weaknessPercentage; } }
        public abstract AttackType resistance { get; }
        public abstract AttackType weakness { get; }
        public abstract AttackType attackType { get; }
        public bool canAttack { get { return this._canAttack; } }
        public bool isAttacking { get { return this._isAttacking; } set { this._isAttacking = value; } }
        #endregion

        #region UNITY
        protected virtual void Awake() {
            this.radiusDrawer.TurnOff();

            this._animator = this.GetComponent<Animator>();
            if(this._animator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            this._agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            this._agent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            var collider = this.GetComponent<CapsuleCollider>();
            this._unitRadius = collider != null ? collider.radius : this.GetComponent<SphereCollider>().radius;
        }

        protected override void OnEnable() {
            base.OnEnable();

            this._hasFinished = false;
            this._canAttack = true;
            this._canMpve = true;
            this._isAttacking = false;
            this._lastPosition = this.transform.position;
        }

        /*protected virtual void Update() {
            this._velocity = (this.position - this._lastPosition);
            this._lastPosition = this.position;
        }*/
        #endregion

        #region CLASS
        //////////////
        //// UNIT ////
        //////////////
        public virtual void NewTurn() {
            this._hasFinished = false;
        }

        public virtual void Finished() {
            this._hasFinished = true;
        }

        //////////////////
        //// MOVEMENT ////
        //////////////////
        public void MoveTo(Vector3 dest) {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.areaMask)) {
                //if((hit.position - this.position).sqrMagnitude > (this._agent.stoppingDistance * this._agent.stoppingDistance))
                    //return; // destination not far enough away.
            }

            this._agent.isStopped = false;
            this._agent.SetDestination(hit.position);
        }

        public void StopMoving() {
            this._agent.isStopped = true;
            this._agent.ResetPath();
        }

        public void LookAt(Vector3 pos) {
            this.transform.LookAt(new Vector3(pos.x, this.transform.position.y, pos.z), Vector3.up);
        }

        ////////////////
        //// ATTACK ////
        ////////////////
        public float GetDamage() {
            return UnityEngine.Random.Range(this._minDamage, this._maxDamage);
        }

        public void Attack(IHasHealth target) {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(target.position);
            this.StopMoving();

            // NOTE: Play attack animation using _animator.

            this.InternalAttack(GetDamage());
        }

        public override bool ReceiveDamage(float damage) {
            if(this.isDead)
                return true;

            this.currentHealth -= damage;

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
            var hits = Utility.Utils.hitsBuffers;
            var pos = this.position + this.transform.forward * this._unitRadius;
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
                hasHealth.ReceiveDamage(damage);
                break;
            }
        }
        #endregion
    }
}