namespace Helpers {

    using UnityEngine;

    using Enum;
    using Player;
    using UI;
    using Utility;

    public abstract class HasHealthBase : EntityBase, IHasHealth {
        #region VARIABLE

        [Header("ENTITY ATTRIBUTES")]
        [SerializeField, ReadOnly] private Player _controller;
        [SerializeField] protected UIBase _uiComponent;
        public Player controller { get { return this._controller; } set { this._controller = value; } }
        public UIBase uiComponent { get { return this._uiComponent; } set { this._uiComponent = value; } }

        [Header("HEALTH")]
        [SerializeField] protected float _currentHealth = 0.0f; 
        [SerializeField] protected float _maxHealth = 100.0f;
        [SerializeField] protected float _currentEnergy = 0.0f;
        [SerializeField] protected float _maxEnergy = 0.0f;

        public float CurrentHealth { get { return this._currentHealth; } }
        public float CurrentEnergy { get { return this._currentEnergy; } }
        public float MaxHealth { get { return this._maxHealth; } }
        public float MaxEnergy { get { return this._maxEnergy; } }
        public bool isDead { get { return this.CurrentHealth <= 0.0f || (this.gameObject != null && !this.isActive); } }

        protected IHasHealth _lastAttacker;
        private float _lastAttacked;
        public float lastAttacked { get { return this._lastAttacked; } }
        public IHasHealth lastAttacker { get { return this._lastAttacker; }
            set { this._lastAttacker = value;
                  this._lastAttacked = Time.timeSinceLevelLoad; } }
        #endregion

        #region UNITY
        protected override void OnEnable() {
            base.OnEnable();

            this._currentHealth = this._maxHealth;

            this._currentEnergy = this._maxEnergy;

            this.uiComponent = this.transform.GetComponent<UIBase>();
            // NOTE: UI INSTANCES HERE e.g health bar and ui buttons
        }
        #endregion

        #region CLASS
        public abstract bool ReceiveDamage(float damage, IHasHealth target);
        public abstract bool ReceiveDamage(float damage, IHasHealth target, Vector3 origin);

        public virtual bool AddHealth(float amount) {
            if(this.isDead)
                return false;

            // Make sure that the amount doesn't exceed the totally maxhealth of the unit.
            if(amount > this.MaxHealth)
                this._currentHealth += amount;
            else
                this._currentHealth = this.MaxHealth;

            this.uiComponent.UpdateUI();
            return true;
        }

        public virtual bool RemoveHealth(float amount) {
            if(this.isDead)
                return false;

            this._currentHealth -= amount;

            if(this.CurrentHealth <= 0.0f)
                this._currentHealth = 0.0f;

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