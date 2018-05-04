namespace Unit {

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using UI;

    [RequireComponent(typeof(MageUI))]
    public sealed class Mage : UnitBase {

        private float _splashRadius;

        public float splashRadius { get { return this._splashRadius; } }

        public override UnitType unitType { get { return UnitType.MAGE; } }

        public override MovementType movementType { get { return UnitValues.Mage.MOVETYPE; } }

        public override AttackType attackType { get { return UnitValues.Mage.ATTACKTYPE; } }
        public override AttackType resistanceType { get { return UnitValues.Mage.RESISTANCETYPE; } }
        public override AttackType weaknessType { get { return UnitValues.Mage.WEAKNESSTYPE; } }

        protected override void OnEnable() {

            this._splashRadius = UnitValues.Mage.SPLASHRADIUS;

            this.currentHealth = UnitValues.Mage.HEALTH;
            this._maxHealth = UnitValues.Mage.HEALTH;
            this.currentEnergy = UnitValues.Mage.ENERGY;
            this._maxEnergy = UnitValues.Mage.ENERGY;

            this._moveSpeed = UnitValues.Mage.MOVESPEED;
            this._moveRadius = UnitValues.Mage.MOVERADIUS;

            this._minDamage = UnitValues.Mage.MINDAMAGE;
            this._maxDamage = UnitValues.Mage.MAXDAMAGE;
            this._attackRadius = UnitValues.Mage.ATTACKRADIUS;

            this._resistancePercentage = UnitValues.Mage.RESISTANCEPERCENTAGE;
            this._weaknessPercentage = UnitValues.Mage.WEAKNESSPERCENTAGE;

            base.OnEnable();
        }

        protected override void InternalAttack(float damage, IHasHealth target) {
            base.InternalAttack(damage, target);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
        }
    }
}