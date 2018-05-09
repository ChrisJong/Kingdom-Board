namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(ClericUI))]
    public sealed class Cleric : UnitBase {

        #region VARIABLE
        private float _healingRadius;
        private float _healingAmount;
        private bool _canHeal = true;
        private UnitState _state = UnitState.NONE;

        public float healingRadius { get { return this._healingRadius; } }
        public float healingAoumt { get { return this._healingAmount; } }
        public bool canHeal { get { return this._canHeal; } }
        public UnitState state { get { return this._state; } }

        public override UnitType unitType { get { return UnitType.CLERIC; } }

        public override MovementType movementType { get { return UnitValues.Cleric.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Cleric.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Cleric.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Cleric.WEAKNESSTYPE; } }
        #endregion

        #region UNITY
        protected override void OnEnable() {

            this._healingRadius = UnitValues.Cleric.HEALINGRADIUS;
            this._healingAmount = UnitValues.Cleric.HEALINGAMOUNT;
            this._canHeal = true;

            this.currentHealth = UnitValues.Cleric.HEALTH;
            this._maxHealth = UnitValues.Cleric.HEALTH;
            this.currentEnergy = UnitValues.Cleric.ENERGY;
            this._maxEnergy = UnitValues.Cleric.ENERGY;

            this._moveSpeed = UnitValues.Cleric.MOVESPEED;
            this._moveRadius = UnitValues.Cleric.MOVERADIUS;

            this._minDamage = UnitValues.Cleric.MINDAMAGE;
            this._maxDamage = UnitValues.Cleric.MAXDAMAGE;
            this._attackRadius = UnitValues.Cleric.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Cleric.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Cleric.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }
        #endregion

        #region CLASS
        public override void NewTurn() {
            base.NewTurn();
            this._canHeal = true;
        }

        //////////////////
        //// MOVEMENT ////
        //////////////////

        /////////////////
        //// HEALING ////
        /////////////////
        public void FinishHealing() {
            this._canHeal = false;
        }

        public void Heal(IHasHealth target) {
            this.LookAt(target.position);
            
            this._animator.Play("Cast");

            this.InternalHeal(target);

            ((UI.ClericUI)this._uiComponent).FinishHeal();
            this._canHeal = false;
        }

        public void InternalHeal(IHasHealth target) {
            var hits = Utils.hitsBuffers;
            //var pos = this.position + this.transform.forward * this._unitRadius;
            var pos = target.position;
            Physics.SphereCastNonAlloc(pos, this._unitRadius * 2.0f, this.transform.forward, hits, this._healingRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);

            this._hitComparer.position = this.position;
            Array.Sort(hits, this._hitComparer);

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                //if(hit.transform == this.transform)
                    //continue; // jgnore hits with itself;

                var hasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hasHealth == null || hasHealth.isDead)
                    continue; // ignore anything that doesn't contain health or is dead.

                if(this.IsEnemy(hasHealth))
                    continue; // ignore enemies.

                // only attack the original target chosen.
                hasHealth.AddHealth(this._healingAmount);
                break;
            }
        }
        #endregion
    }
}