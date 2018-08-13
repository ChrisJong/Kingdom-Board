namespace Helpers {

    using UnityEngine;

    using Enum;
    using Player;
    using UI;
    using Utility;

    public abstract class HasHealthBase : EntityBase, IHasHealth {
        #region VARIABLE
        [Header("HEALTH")]
        [SerializeField, ReadOnly]
        private Player _controller;
        [SerializeField, Range(0.0f, 500.0f)]
        protected float _maxHealth = 100.0f;
        [SerializeField, ReadOnly]
        protected float _debugCurrentHealth = 0.0f;
        [SerializeField, Range(0.0f, 500.0f)]
        protected float _maxEnergy = 0.0f;
        [SerializeField, ReadOnly]
        protected float _debugCurrentEnergy = 0.0f;
        protected UIBase _uiComponent;
        protected IHasHealth _lastAttacker;
        private float _lastAttacked;

        public float maxHealth { get { return this._maxHealth; } }
        public float maxEnergy { get { return this._maxEnergy; } }

        public float currentHealth { get; set; }
        public float currentEnergy { get; set; }

        public bool isDead { get { return this.currentHealth <= 0.0f || (this.gameObject != null && !this.isActive); } }

        public float lastAttacked { get { return this._lastAttacked; } }

        public IHasHealth lastAttacker {
            get { return this._lastAttacker; }
            set { this._lastAttacker = value;
                  this._lastAttacked = Time.timeSinceLevelLoad; } }

        public UIBase uiComponent {
            get { return this._uiComponent; }
            set { this._uiComponent = value; } }

        public Player controller {
            get { return this._controller; }
            set { this._controller = value; } }
        #endregion

        #region UNITY
        protected override void OnEnable() {
            base.OnEnable();

            this.currentHealth = this._maxHealth;
            this._debugCurrentHealth = this._maxHealth;

            this.currentEnergy = this._maxEnergy;
            this._debugCurrentEnergy = this._maxEnergy;

            this.uiComponent = this.transform.GetComponent<UIBase>();
            // NOTE: UI INSTANCES HERE e.g health bar and ui buttons
        }
        #endregion

        #region CLASS
        public abstract bool ReceiveDamage(float damage, IHasHealth target);

        public virtual bool AddHealth(float amount) {
            if(this.isDead)
                return false;

            // Make sure that the amount doesn't exceed the totally maxhealth of the unit.
            if(amount > this.maxHealth)
                this.currentHealth += amount;
            else
                this.currentHealth = this.maxHealth;

            this._debugCurrentHealth = this.currentHealth;
            this.uiComponent.UpdateUI();
            return true;
        }

        public virtual bool RemoveHealth(float amount) {
            if(this.isDead)
                return false;

            this.currentHealth -= amount;

            if(this.currentHealth <= 0.0f)
                this.currentHealth = 0.0f;

            this._debugCurrentHealth = this.currentHealth;
            this.uiComponent.UpdateUI();
            return true;
        }

        public virtual bool UseEnergy(float amount) {
            return true;
        }

        public virtual bool IsAlly(IHasHealth other) {
            return ReferenceEquals(this.controller, other.controller);
        }

        public virtual bool IsEnemy(IHasHealth other) {
            return !this.IsAlly(other);
        }
        #endregion
    }
}