namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(ClericUI))]
    public sealed class Cleric : UnitBase {

        private float _splashRadius;

        public float splashRadius { get { return this._splashRadius; } }

        public override UnitType unitType { get { return UnitType.CLERIC; } }

        public override MovementType movementType { get { return UnitValues.Cleric.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Cleric.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Cleric.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Cleric.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this._splashRadius = UnitValues.Cleric.SPLASHRADIUS;

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

        public void Heal(IHasHealth target) {

        }

        public void InternalHeal(float amount, IHasHealth target) {

        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            base.InternalAttack(damage, target);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
        }
    }
}