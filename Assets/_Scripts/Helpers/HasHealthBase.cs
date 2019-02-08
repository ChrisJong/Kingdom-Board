namespace Helpers {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Player;
    using UI;

    public abstract class HasHealthBase : EntityBase, IHasHealth {
        #region VARIABLE

        [SerializeField] private Player _controller;
        [SerializeField] private UIBase _uiBase;
        protected IHasHealth _lastAttacker;
        [SerializeField] protected List<HasHealthBase> _lastAttackers = new List<HasHealthBase>(); // NOTE Change to IHasHealthBase Later.

        [Header("ENTITY - HEALTH & ENERGY")]
        [SerializeField] protected float _currentHealth = 0.0f;
        [SerializeField] protected float _maxHealth = 0.0f;
        [SerializeField] protected float _currentEnergy = 0.0f;
        [SerializeField] protected float _maxEnergy = 0.0f;
        private float _lastAttacked = 0.0f;

        public float CurrentHealth { get { return this._currentHealth; } }
        public float CurrentEnergy { get { return this._currentEnergy; } }
        public float MaxHealth { get { return this._maxHealth; } }
        public float MaxEnergy { get { return this._maxEnergy; } }
        public float LastAttacked { get { return this._lastAttacked; } }

        public bool IsDead { get { return this._currentHealth <= 0.0f || (this.gameObject != null && !this.isActive); } }

        public Player Controller { get { return this._controller; } }
        public UIBase uiBase { get { return this._uiBase; } }
        public IHasHealth LastAttacker {
            get { return this._lastAttacker; }
            set { this._lastAttacker = value;
                  this._lastAttackers.Add(value as HasHealthBase);
                  this._lastAttacked = Time.timeSinceLevelLoad; }
        }

        #endregion

        #region CLASS

        public override void Setup() {
            this._uiBase = this.transform.GetComponent<UIBase>() as UIBase;

            this.IsSetup = true;

            this._currentHealth = this._maxHealth;
            this._currentEnergy = this._maxEnergy;
        }

        public override void Init() {
            this._currentHealth = this._maxHealth;
            this._currentEnergy = this._maxEnergy;
        }

        public virtual void Init(Player contoller) {
            this.Init();

            this._controller = contoller;

            if(this._uiBase != null)
                this._uiBase.Init(contoller);
        }

        public override void Return() {
            this._lastAttacker = null;
            this._lastAttackers.Clear();

            this._controller = null;
        }

        public abstract bool ReceiveDamage(float damage, IHasHealth target);
        public abstract bool ReceiveDamage(float damage, IHasHealth target, Vector3 origin);

        public virtual bool AddHealth(float amount) {
            if(this.IsDead)
                return false;

            this._currentHealth += amount;

            // Make sure that the amount doesn't exceed the totally maxhealth of the unit.
            if(this._currentHealth > this._maxHealth)
                this._currentHealth = this._maxHealth;

            this.uiBase.UpdateUI();
            return true;
        }

        public virtual bool RemoveHealth(float amount) {
            if(this.IsDead)
                return false;

            this._currentHealth -= amount;

            if(this.CurrentHealth <= 0.0f)
                this._currentHealth = 0.0f;

            this.uiBase.UpdateUI();
            return true;
        }

        public virtual bool UseEnergy(float amount) {
            return true;
        }

        public virtual bool IsAlly(IHasHealth other) {
            return ReferenceEquals(this.Controller, other.Controller);
        }

        public virtual bool IsAlly(Player controller) {
            if(this._controller.id == controller.id)
                return true;
            else
                return false;
        }

        public virtual bool IsEnemy(IHasHealth other) {
            return !this.IsAlly(other);
        }

        public virtual bool IsEnemy(Player controller) {
            return !this.IsAlly(controller);
        }
        #endregion
    }
}