﻿namespace Unit {

    //using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Helpers;
    using System;

    public abstract class UnitBase : HasHealthBase, IUnit {

        #region VARIABLE
        ////////////
        /// UNIT ///
        ////////////
        protected Animator _animator;
        protected float _unitRadius;
        private UnityEngine.AI.NavMeshAgent _agent;

        public abstract UnitType unitType { get; }
        public override EntityType entityType {
            get { return EntityType.UNIT; }
        }
        public LayerMask areaMask {
            get { return this._agent.areaMask; }
        }

        ////////////////
        /// MOVEMENT ///
        ////////////////
        protected float _minMoveThreashold = 0.01f;
        private float _moveSpeed = 5.0f;
        private float _moveRadius = 10.0f;
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        public bool isMoving {
            get { return this._velocity.sqrMagnitude > (this._minMoveThreashold * this._minMoveThreashold); }
        }
        public virtual bool isIdle {
            get { return !this.isMoving && !this.isDead; }
        }
        public float moveSpeed {
            get { return this._moveSpeed; }
        }
        public float moveRadius {
            get { return this._moveRadius; }
        }

        //////////////
        /// ATTACK ///
        //////////////
        protected float _minDamage = 10.0f;
        protected float _maxDamage = 20.0f;
        protected float _attackRadius = 7.5f;
        protected float _aoeAttackRadius = 3.0f;
        protected float _lastAttack;

        public float minDamage {
            get { return this._minDamage; }
        }
        public float maxDamage {
            get { return this._maxDamage; }
        }
        public float attackRadius {
            get { return this._attackRadius; }
        }
        public float aoeAttackRadius {
            get { return this._aoeAttackRadius; }
        }
        #endregion

        #region UNITY
        protected virtual void Awake() {
            this._animator = this.GetComponent<Animator>();
            if(this._animator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            this._agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            this._agent.avoidancePriority = UnityEngine.Random.Range(0, 99);
        }
        #endregion

        #region CLASS
        ////////////////
        /// MOVEMENT ///
        ////////////////
        public void MoveTo(Vector3 dest) {

        }

        public void StopMoving() {
            
        }

        public void LookAt(Vector3 pos) {
            this.transform.LookAt(new Vector3(pos.x, this.transform.position.y, pos.z), Vector3.up);
        }

        //////////////
        /// ATTACK ///
        //////////////
        public float GetDamage() {
            return UnityEngine.Random.Range(this._minDamage, this._maxDamage);
        }

        public void Attack(IHasHealth target) {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(target.position);

            // NOTE: Play attack animation using _animator.

            this.InternalAttack(GetDamage());
        }

        public void AttackAOE(IHasHealth target) {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(target.position);

            this.InternalAttack(GetDamage(), true);
        }

        public override bool ReceiveDamage(float damage) {
            if(this.isDead)
                return true;

            this.currentHealth -= damage;

            if(this.currentHealth <= 0.0f) {
                if(this.controller != null) {
                    Debug.Log("remove this unit: " + this.name);
                    // NOTE: Remove this unity from the player controller list.
                }

                // NOTE: return this to the unitpoolmanager for later use.

                return true;
            } else {
                // NOTE: play hit anbimation.
            }
            return false;
        }

        protected virtual void InternalAttack(float damage, bool aoe = false) {

        }
        #endregion
    }
}